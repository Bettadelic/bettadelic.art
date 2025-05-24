@Library( "X13JenkinsLib" )_

final String IpfsUploaderVersion = "1.2.0";

// Apparently this means get all files within the subfolders
// of public, but not the files within public.
final String fileGlob = "checkout/static/bettas/*.jpg";

pipeline
{
    parameters
    {
        string(
            name: "agent",
            defaultValue: "master",
            description: "The agent label to pass in"
        );
        string(
            name: "ipfsServerUrl",
            defaultValue: "127.0.0.1",
            description: "The URL to the server that is running the IPFS node"
        );
        string(
            name: "ipfsPort",
            defaultValue: "5001",
            description: "The port of the IPFS server that is running the IPFS node's API port"
        );
    }
    agent
    {
        label params.agent;
    }
    environment
    {
        DOTNET_CLI_TELEMETRY_OPTOUT = 'true'
        DOTNET_NOLOGO = 'true'
    }
    options
    {
        skipDefaultCheckout( true );
    }
    stages
    {
        stage( 'clean' )
        {
            steps
            {
                cleanWs();
            }
        }
        stage( 'checkout' )
        {
            steps
            {
                checkout scm;
            }
        }
        stage( 'In Docker' )
        {
            agent
            {
                docker
                {
                    image 'mcr.microsoft.com/dotnet/sdk:8.0'
                    args "-e HOME='${env.WORKSPACE}'"
                    reuseNode true
                }
            }
            stages
            {
                stage( 'Install Uploader' )
                {
                    steps
                    {
                        sh "dotnet tool update IpfsUploader.Tool --version ${IpfsUploaderVersion} --tool-path ./IpfsUploader";
                    }
                }
                stage( 'Add Files' )
                {
                    steps
                    {
                        sh "./IpfsUploader/ipfs_upload --server_url='${params.ipfsServerUrl}' --port='${params.ipfsPort}' --file='${pwd()}/${fileGlob}' --output_xml_file='${pwd()}/archive_files_ipfs.xml'";
                        archiveArtifacts "archive_files_ipfs.xml";
                    }
                }
            }
        }
    }
    post
    {
        fixed
        {
            X13SendToTelegramWithCredentials(
                message: "${BUILD_TAG} has been fixed!  See: ${BUILD_URL}",
                botCredsId: "telegram_bot",
                chatCredsId: "telegram_chat_id"
            );
        }
        failure
        {
            X13SendToTelegramWithCredentials(
                message: "${BUILD_TAG} has failed.  See: ${BUILD_URL}",
                botCredsId: "telegram_bot",
                chatCredsId: "telegram_chat_id"
            );
        }
    }
}

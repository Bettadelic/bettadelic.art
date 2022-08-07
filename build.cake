// ---------------- Constants ----------------

string target = Argument( "target", "taste" );

const string pretzelExe = "./_pretzel/src/Pretzel/bin/Debug/net6.0/Pretzel.dll";
const string pluginDir = "./_plugins";
const string categoryPlugin = "./_plugins/Pretzel.Categories.dll";
const string extensionPlugin = "./_plugins/Pretzel.SethExtensions.dll";

DirectoryPath nodeModulesDir = Directory( "_store/node_modules" );
DirectoryPath compiledTsDir = Directory( "static/compiled_ts" );
FilePath storeJsFile = compiledTsDir.CombineWithFilePath( File( "Store.js" ) );

DirectoryPath siteDir = Directory( "_site" );

ushort port = Argument<ushort>( "port", 8080 );

// ---------------- Tasks ----------------

Task( "taste" )
.Does(
    () =>
    {
        RunPretzel( "taste", false );
    }
).Description( "Calls pretzel taste to try the site locally" );

Task( "generate" )
.Does(
    () =>
    {
        EnsureDirectoryExists( siteDir );
        CleanDirectory( siteDir );
        RunPretzel( "bake", true );
    }
).Description( "Builds the site for publishing." );

Task( "npm_install" )
.Does(
    () =>
    {
        NpmInstall();
    }
);

Task( "build_store" )
.Does(
    () =>
    {
        BuildStore();
    }
).Description( "Builds the TypeScript Store app." );

Task( "build_pretzel" )
.Does(
    () =>
    {
        BuildPretzel();
    }
).Description( "Compiles Pretzel" );

Task( "build_all" )
.IsDependentOn( "build_store" )
.IsDependentOn( "build_pretzel" )
.IsDependentOn( "taste" );

// ---------------- Functions  ----------------

void CheckForNodeModules()
{
    if( DirectoryExists( nodeModulesDir ) == false )
    {
        NpmInstall();
    }
}

void NpmInstall()
{
    Information( "NPM Restore..." );
    EnsureDirectoryExists( compiledTsDir );
    CleanDirectory( compiledTsDir );

    FilePath npmPath;
    if( IsRunningOnWindows() )
    {
        npmPath = Context.Tools.Resolve( "npm.cmd" );
    }
    else
    {
        npmPath = Context.Tools.Resolve( "npm" );
    }

    var processSettings = new ProcessSettings
    {
        Arguments = new ProcessArgumentBuilder()
            .Append( "install" ),
        WorkingDirectory = "_store"
    };

    int exitCode = StartProcess( npmPath, processSettings );
    if( exitCode != 0 )
    {
        throw new CakeException( "NPM restore failed!.  Got exit code: "  + exitCode );
    }

    Information( "NPM Restore... Done!" );
}

void CheckStoreDependency()
{
    if( FileExists( storeJsFile ) == false )
    {
        CheckForNodeModules();
        BuildStore();
    }
}

void BuildStore()
{
    Information( "Building Store..." );
    EnsureDirectoryExists( compiledTsDir );
    CleanDirectory( compiledTsDir );

    FilePath nodePath;
    if( IsRunningOnWindows() )
    {
        nodePath = Context.Tools.Resolve( "node.exe" );
    }
    else
    {
        nodePath = Context.Tools.Resolve( "node" );
    }

    var processSettings = new ProcessSettings
    {
        Arguments = new ProcessArgumentBuilder()
            .Append( "node_modules/webpack-cli/bin/cli.js" ),
        WorkingDirectory = "_store"
    };

    int exitCode = StartProcess( nodePath, processSettings );
    if( exitCode != 0 )
    {
        throw new CakeException( "Could not build store!  Got exit code: "  + exitCode );
    }

    Information( "Building Store... Done!" );
}

void BuildPretzel()
{
    Information( "Building Pretzel..." );

    DotNetBuildSettings settings = new DotNetBuildSettings
    {
        Configuration = "Debug"
    };

    DotNetBuild( "./_pretzel/src/Pretzel.sln", settings );

    EnsureDirectoryExists( pluginDir );

    // Move Pretzel.Categories.
    {
        FilePathCollection files = GetFiles( "./_pretzel/src/Pretzel.Categories/bin/Debug/net6.0/Pretzel.Categories.*" );
        CopyFiles( files, Directory( pluginDir ) );
    }

    // Move Pretzel.SethExtensions
    {
        FilePathCollection files = GetFiles( "./_pretzel/src/Pretzel.SethExtensions/bin/Debug/net6.0/Pretzel.SethExtensions.*" );
        CopyFiles( files, Directory( pluginDir ) );
    }

    Information( "Building Pretzel... Done!" );
}

void RunPretzel( string argument, bool abortOnFail )
{
    CheckStoreDependency();
    CheckPretzelDependency();

    bool fail = false;
    string onStdOut( string line )
    {
        if( string.IsNullOrWhiteSpace( line ) )
        {
            return line;
        }
        else if( line.StartsWith( "Failed to render template" ) )
        {
            fail = true;
        }

        Console.WriteLine( line );

        return line;
    }

    ProcessSettings settings = new ProcessSettings
    {
        Arguments = ProcessArgumentBuilder.FromString( $"\"{pretzelExe}\" {argument} --debug -p {port}" ),
        Silent = false,
        RedirectStandardOutput = abortOnFail,
        RedirectedStandardOutputHandler = onStdOut
    };

    int exitCode = StartProcess( "dotnet", settings );
    if( exitCode != 0 )
    {
        throw new Exception( $"Pretzel exited with exit code: {exitCode}" );
    }

    if( abortOnFail && fail )
    {
        throw new Exception( "Failed to render template" );   
    }
}

void CheckPretzelDependency()
{
    if(
        ( FileExists( pretzelExe ) == false ) ||
        ( FileExists( categoryPlugin ) == false ) ||
        ( FileExists( extensionPlugin ) == false )
    )
    {
        BuildPretzel();
    }
}

RunTarget( target );

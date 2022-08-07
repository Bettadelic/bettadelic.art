import WalletConnect from "@walletconnect/client";
import QRCodeModal from "algorand-walletconnect-qrcode-modal";
import { IInternalEvent } from "@walletconnect/types";
import { formatJsonRpcRequest } from "@json-rpc-tools/utils";
//import algosdk from "algosdk";

export class Store
{
    // ---------------- Fields ----------------

    readonly connector: WalletConnect;

    // ---------------- Constructor  ----------------

    constructor()
    {
        const bridge = "https://bridge.walletconnect.org";
        this.connector = new WalletConnect( { bridge, qrcodeModal: QRCodeModal } );
    }

    // ---------------- Functions  ----------------

    public async ConnectWallet()
    {
        if(!this.connector.connected) {
            await this.connector.createSession();
        }

        this.connector.on( "connect", this.Connector_Connect );
        this.connector.on( "disconnect", this.Connector_Disconnect );
        this.connector.on( "session_update", this.Connector_SessionUpdate );

        console.log("Connected!");
    }

    public async KillSession()
    {
        if(!this.connector.connected) {
            return;
        }

        this.connector.killSession();
    }

    // ---------------- Event Handlers  ----------------

    private async Connector_Connect( error: any, payload: any )
    {
        if( error )
        {
            throw error;
        }
    }

    private async Connector_Disconnect( error: any, payload: any )
    {
        if( error )
        {
            throw error;
        }
    }

    private async Connector_SessionUpdate( error: any, payload: any )
    {
        if( error )
        {
            throw error;
        }
    }
}

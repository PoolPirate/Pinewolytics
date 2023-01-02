import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import type { TerraValidatorCountDTO } from '$lib/models/TerraDTOs';

export enum QueryName {
    TerraValidatorCountHistory = "terra-validator-count-history",
}

const queryTypes = {
    [QueryName.TerraValidatorCountHistory]: { } as TerraValidatorCountDTO[],
}

interface QuerySubscription {
    queryName: QueryName;
    handler: ((arg0: any) => void);
}

export class QuerySubscriptionBuilder {
    private connection: HubConnection;
    
    private subscriptions: QuerySubscription[];

    constructor() {
        this.subscriptions = [];
        this.connection = new HubConnectionBuilder()
            .withUrl('/api/hub/query')
            .withAutomaticReconnect()
            .build();

        this.connection.on("SendQueryResult", (queryName, result) => {
            this.subscriptions
                .filter(x => x.queryName == queryName)
                .forEach(x => {
                    x.handler(result);
                });
        });
    }

    async start() {
        await this.connection.start();

        this.subscriptions.forEach(async name => {
            await this.connection.send("GetAndSubscribe", name.queryName);
        });
    }

    addQuery<
        T extends QueryName, // <- T points to a key
        R extends (typeof queryTypes)[T] // <- R points to the type of that key
    >(name: T, handler: (data : R) => void) {
        this.subscriptions.push({
            queryName: name,
            handler: handler
        });
        return this;
    }
}
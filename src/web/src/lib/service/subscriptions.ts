import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { writable } from 'svelte/store';
import { browser } from '$app/environment';
import { type QueryName, queryTypes } from './query-definitions';
import { realtimeValueTypes, type RealtimeValueName } from './realtime-value-definitions';
import type RefreshAnimation from '$lib/components/RefreshAnimation.svelte';


interface QuerySubscription {
	name: QueryName | RealtimeValueName;
	handler: (arg0: any) => void;
}

export class SocketSubscriptionBuilder {
	private connection: HubConnection | null;

	private subscriptions: QuerySubscription[];

	constructor() {
		this.subscriptions = [];

		if (!browser) {
			this.connection = null!;
			return;
		}

		this.connection = new HubConnectionBuilder()
			.withUrl('/api/hub/query')
			.withAutomaticReconnect()
			.build();

		this.connection.on('SendQueryResult', (queryName, result) => {
			this.subscriptions
				.filter((x) => x.name == queryName)
				.forEach((x) => {
					x.handler(result);
				});
		});
			this.connection.on('SendRealtimeValue', (queryName, result) => {
			this.subscriptions
				.filter((x) => x.name == queryName)
				.forEach((x) => {
					x.handler(result);
				});
		});
	}

	async start() {
        if (this.connection == null) {
            return;
        }

		await this.connection.start();

		this.subscriptions.forEach(async (subscription) => {
			await this.connection!.send('GetAndSubscribe', subscription.name);
		});
	}

    dispose() {
        if (this.connection == null) {
            return;
        }
        this.connection.stop();
    }

	addQuery<T extends QueryName, R extends typeof queryTypes[T]>(
		name: T,
		handler: (data: R) => void
	) {
		this.subscriptions.push({
			name: name,
			handler: handler
		});
		return this;
	}

	addRealtimeValue<T extends RealtimeValueName, R extends typeof realtimeValueTypes[T]>(
		name: T,
		handler: (data: R) => void
	) {
		this.subscriptions.push({
			name: name,
			handler: handler
		});
		return this;
	}
}

export function createQueryListener<T extends QueryName, R extends typeof queryTypes[T]>(
	builder: SocketSubscriptionBuilder,
	queryName: T
) {
	const { subscribe, set } = writable<R>(queryTypes[queryName] as any);

	builder.addQuery(queryName, (value) => {
		set(value as any);
	});

	return {
		subscribe
	};
}

export function createRealtimeValueListener<T extends RealtimeValueName, R extends typeof realtimeValueTypes[T]>(
	builder: SocketSubscriptionBuilder,
	realtimeValueName: T,
	animationGetter: (() => RefreshAnimation[])
) {
	const { subscribe, set } = writable<R>(realtimeValueTypes[realtimeValueName] as any);

	builder.addRealtimeValue(realtimeValueName, (value) => {
		set(value as any);
		animationGetter().forEach(x => x.play());
	});

	return {
		subscribe
	};
}

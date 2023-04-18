import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { writable } from 'svelte/store';
import { browser } from '$app/environment';
import { type QueryName, queryTypes } from './query-definitions';
import type RefreshAnimation from '$lib/components/RefreshAnimation.svelte';
import type { RealtimeValueName, realtimeValueTypes } from './realtime-value-definitions';
import { getQueryValue, getRealtimeFeedValue, getRealtimeValueValue } from './queries';
import { realtimeFeedLengths, type RealtimeFeedName, type realtimeFeedTypes } from './realtime-feed-definitions';


interface QuerySubscription {
	name: QueryName;
	handler: (arg0: any) => void;
}
interface RealtimeValueSubscription {
	name: RealtimeValueName;
	handler: (arg0: any) => void;
}
interface RealtimeFeedSubscription {
	name: RealtimeFeedName;
	setter: (arg0: any[]) => void;
	handler: (arg0: any) => void;
}

export class SocketSubscriptionBuilder {
	private connection: HubConnection | null;

	private querySubscriptions: QuerySubscription[];
	private realtimeValueSubscriptions: RealtimeValueSubscription[];
	private realtimeFeedSubscriptions: RealtimeFeedSubscription[];

	constructor() {
		this.querySubscriptions = [];
		this.realtimeValueSubscriptions = [];
		this.realtimeFeedSubscriptions = [];

		if (!browser) {
			this.connection = null!;
			return;
		}

		this.connection = new HubConnectionBuilder()
			.withUrl('/api/hub/query')
			.withAutomaticReconnect()
			.build();

		this.connection.on('SendQueryResult', (queryName, result) => {
			this.querySubscriptions
				.filter((x) => x.name == queryName)
				.forEach((x) => {
					x.handler(result);
				});
		});
		this.connection.on('SendRealtimeValue', (valueName, result) => {
			this.realtimeValueSubscriptions
				.filter((x) => x.name == valueName)
				.forEach((x) => {
					x.handler(result);
				});
		});
		this.connection.on('SendRealtimeFeedExtension', (feedName, result) => {
			this.realtimeFeedSubscriptions
				.filter((x) => x.name == feedName)
				.forEach((x) => {
					x.handler(result);
				});
		});
	}

	async start() {
        if (this.connection == null) {
            return;
        }

		this.querySubscriptions.forEach(async (subscription) => {
			const result = await getQueryValue(subscription.name);
			subscription.handler(result.value);
		});
		this.realtimeValueSubscriptions.forEach(async (subscription) => {
			const result = await getRealtimeValueValue(subscription.name);
			subscription.handler(result.value);
		});
		this.realtimeFeedSubscriptions.forEach(async (subscription) => {
			const result = await getRealtimeFeedValue(subscription.name);
			subscription.setter(result.value as any[]);
		});

		this.connection.onreconnected(() => {
			this.sendSubscriptions();
		})

		await this.connection.start();
		await this.sendSubscriptions();
	}

	async sendSubscriptions() {
		this.querySubscriptions.forEach(async (subscription) => {
			await this.connection!.send('Subscribe', subscription.name);
		});
		this.realtimeValueSubscriptions.forEach(async (subscription) => {
			await this.connection!.send('Subscribe', subscription.name);
		});
		this.realtimeFeedSubscriptions.forEach(async (subscription) => {
			await this.connection!.send('Subscribe', subscription.name);
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
		this.querySubscriptions.push({
			name: name,
			handler: handler
		});
		return this;
	}

	addRealtimeValue<T extends RealtimeValueName, R extends typeof realtimeValueTypes[T]>(
		name: T,
		handler: (data: R) => void
	) {
		this.realtimeValueSubscriptions.push({
			name: name,
			handler: handler
		});
		return this;
	}

	addRealtimeFeed<T extends RealtimeFeedName, R extends typeof realtimeFeedTypes[T]>(
		name: T,
		handler: (data: R) => void,
		setter: (data: R[]) => void
	) {
		this.realtimeFeedSubscriptions.push({
			name: name,
			handler: handler,
			setter: setter
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
	const { subscribe, set } = writable<R | null>(null);

	builder.addRealtimeValue(realtimeValueName, (value) => {
		set(value as any);
		animationGetter().forEach(x => x.play());
	});

	return {
		subscribe
	};
}

export function createRealtimeFeedListener<T extends RealtimeFeedName, R extends typeof realtimeFeedTypes[T]>(
	builder: SocketSubscriptionBuilder,
	realtimeFeedName: T
) {
	const { subscribe, set, update } = writable<R[] | null>(null);

	builder.addRealtimeFeed(realtimeFeedName, (value) => {
		update(current => {
			if (current == null) {
				console.log([value as R]);
				return [value as R];
			}

			current = [value as R, ...current];

			if (current.length > realtimeFeedLengths[realtimeFeedName]) {
				return current.slice(0, current.length - 1);
			}

			return current;
		});
	}, (value) => {
		set(value as R[]);
	});

	return {
		subscribe
	};
}

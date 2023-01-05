import type { number } from "echarts";

export interface TimeSeriesEntry {
    timestamp: Date;
    value: number;
}

export function DaySeriesToWeekSeries(series: TimeSeriesEntry[], combinator: (dailyValues: number[]) => number) {
    const outputSeries: TimeSeriesEntry[] = [];

    var currentWeek: Date = series[0].timestamp;
    var currentValues: number[] = [];

    series.forEach(value => {
        if (value.timestamp.getDay() == 0) {
            if (currentValues.length != 0){
                outputSeries.push({
                    timestamp: currentWeek,
                    value: combinator(currentValues)
                });
            }

            currentWeek = value.timestamp; 
            currentValues = [];
        }

        currentValues.push(value.value);
    });

    outputSeries.push({
        timestamp: currentWeek,
        value: combinator(currentValues)
    });

    return outputSeries;
}

export function DaySeriesToWeekSeriesBySum(series: TimeSeriesEntry[]) {
    return DaySeriesToWeekSeries(series, vals => vals.reduce((curr, val) => curr + val, 0));
}

export function DaySeriesToWeekSeriesByAvg(series: TimeSeriesEntry[]) {
    return DaySeriesToWeekSeries(series, vals => vals.reduce((curr, val, _, arr) => curr + val / arr.length, 0));
}

export function DaySeriesToWeekSeriesByLast(series: TimeSeriesEntry[]) {
    return DaySeriesToWeekSeries(series, vals => vals[vals.length - 1]);
}

export function DaySeriesToWeekSeriesByMax(series: TimeSeriesEntry[]) {
    return DaySeriesToWeekSeries(series, vals => Math.max(...vals));
}

export function DaySeriesToWeekSeriesByMin(series: TimeSeriesEntry[]) {
    return DaySeriesToWeekSeries(series, vals => Math.min(...vals));
}
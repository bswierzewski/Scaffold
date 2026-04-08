import { createFileRoute } from "@tanstack/react-router";

import { useGetWeatherForecast } from "../api/scaffold-api/scaffold-api";

export const Route = createFileRoute("/")({ component: App });

function App() {
	const { data, error, isLoading, isFetching } = useGetWeatherForecast();

	if (isLoading) {
		return (
			<main>
				<h1>Weather forecast</h1>
				<p>Loading forecast...</p>
			</main>
		);
	}

	if (error) {
		return (
			<main>
				<h1>Weather forecast</h1>
				<p>Could not load forecast data.</p>
			</main>
		);
	}

	return (
		<main>
			<h1>Weather forecast</h1>
			{isFetching ? <p>Refreshing data...</p> : null}
			<ul>
				{data?.map((forecast) => (
					<li key={forecast.date}>
						<strong>{new Date(forecast.date).toLocaleDateString()}</strong>
						{" - "}
						<span>{forecast.temperatureC}C</span>
						{forecast.temperatureF ? <span>{" / "}{forecast.temperatureF}F</span> : null}
						{forecast.summary ? <span>{" - "}{forecast.summary}</span> : null}
					</li>
				))}
			</ul>
		</main>
	);
}

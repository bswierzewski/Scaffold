import { defineConfig } from "orval";

export default defineConfig({
	api: {
		input: {
			target: "../../../openapi/Scaffold.Api.json",
		},
		output: {
			biome: true,
			mode: "tags-split",
			target: "src/api/generated.ts",
			schemas: "src/api/models",
			client: "react-query",
			httpClient: "axios",
			mock: false,
			tsconfig: "./tsconfig.app.json",
			override: {
				mutator: {
					path: "src/api/axios-instance.ts",
					name: "customInstance",
				},
			},
		},
	}
});

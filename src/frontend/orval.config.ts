import { defineConfig } from "orval";

export default defineConfig({
	api: {
		input: {
			target: "http://localhost:7000/openapi/v1.json",
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

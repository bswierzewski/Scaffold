import axios, { type AxiosRequestConfig } from "axios";

export const AXIOS_INSTANCE = axios.create();

// Response interceptor for error handling
AXIOS_INSTANCE.interceptors.response.use(
	(response) => response,
	(error) => {
		// You can add custom error handling here
		// e.g., redirect to login on 401, show toast notifications, etc.
		return Promise.reject(error);
	},
);

export const customInstance = <T>(
	config: AxiosRequestConfig,
	options?: AxiosRequestConfig,
): Promise<T> => {
	return AXIOS_INSTANCE({
		...config,
		...options,
	}).then(({ data }) => data);
};

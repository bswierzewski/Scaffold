# ==========================================
# Stage 1: Build the application (Node.js)
# ==========================================
FROM node:22-alpine AS builder

# Set the working directory
WORKDIR /app

# Copy dependency files to maximize layer caching
COPY package*.json ./

# Install dependencies from the lockfile
RUN npm ci

# Copy the rest of the source code
COPY . .

# Build the Vite application
RUN npm run build

# ==========================================
# Stage 2: Serve files (Nginx)
# ==========================================
FROM nginx:1.29-alpine

# Copy custom Nginx configuration for SPA routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the built application into the Nginx web root
COPY --from=builder /app/dist /usr/share/nginx/html

CMD ["nginx", "-g", "daemon off;"]
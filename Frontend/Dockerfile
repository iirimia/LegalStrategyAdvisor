# Build stage
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files first for better caching
COPY package*.json ./

# Install dependencies with latest npm
RUN npm install --legacy-peer-deps

# Copy source code
COPY . .

# Build for production with latest optimizations
RUN npm run build

# Production stage with nginx
FROM nginx:alpine

# Copy built application (update path for latest Angular CLI)
COPY --from=build /app/dist/legal-strategy-advisor/browser /usr/share/nginx/html

# Create nginx config for Angular SPA
RUN echo 'events { worker_connections 1024; } \
    http { \
    include /etc/nginx/mime.types; \
    default_type application/octet-stream; \
    server { \
    listen 4200; \
    server_name localhost; \
    root /usr/share/nginx/html; \
    index index.html; \
    location / { \
    try_files $uri $uri/ /index.html; \
    } \
    location /api/ { \
    proxy_pass http://api:5199/; \
    proxy_set_header Host $host; \
    proxy_set_header X-Real-IP $remote_addr; \
    } \
    } \
    }' > /etc/nginx/nginx.conf

EXPOSE 4200

CMD ["nginx", "-g", "daemon off;"]
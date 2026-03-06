# Use Node.js 20 Alpine as base image
FROM node:20-alpine

# Install dependencies
RUN apk add --no-cache libc6-compat
WORKDIR /app

# Copy package files
COPY package.json package-lock.json* ./

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build arguments for API URLs
ARG VITE_API_BASE_URL=https://api-worklines.wixisoftware.com
ARG VITE_API_URL=https://api-worklines.wixisoftware.com/api

# Set environment variables for build
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_API_URL=$VITE_API_URL

# Build the application
RUN npm run build

# Expose port 5500
EXPOSE 5500

# Start the application
CMD ["npx", "vite", "preview", "--port", "5500", "--host", "0.0.0.0"]


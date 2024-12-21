# Stage 1: Build the Angular app
FROM node:20 AS build
WORKDIR /app
COPY prema.shuleone.web.client/package*.json ./
RUN npm install
COPY prema.shuleone.web.client/ ./
RUN npm run build --prod

# Stage 2: Serve the app with Nginx
FROM nginx:alpine
COPY --from=build /app/dist/prema.shuleone.web.client/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

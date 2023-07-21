# escape=`

ARG BASE_IMAGE=node:18-alpine

# Build stages below only used for production builds
# Install  dependencies
FROM  ${BASE_IMAGE} AS dependencies
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci

# Rebuild the source code only when needed
FROM  ${BASE_IMAGE} AS builder
WORKDIR /app
COPY . .
COPY --from=dependencies /app/node_modules ./node_modules

RUN npm run build

# Production image, copy all the files and run next
FROM  ${BASE_IMAGE} AS production
WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app/next.config.js ./
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./package.json
COPY --from=builder /app/tsconfig.json ./tsconfig.json

EXPOSE 3000

RUN npx next telemetry disable

ENTRYPOINT ["npm", "run", "start"]

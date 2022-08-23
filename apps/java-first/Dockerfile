# Build
FROM maven:3.6.0-jdk-11-slim AS build
COPY src /home/app/src
COPY pom.xml /home/app
RUN mvn -f /home/app/pom.xml clean package

# Package stage
FROM openjdk:11-jre-slim
COPY --from=build /home/app/target/java-first-0.0.1-SNAPSHOT.jar /usr/local/lib/java-first.jar

ADD opentelemetry/opentelemetry.jar /usr/local/lib

ARG otelExporterOtlpEndpoint

ENV OTEL_SERVICE_NAME=java-first
ENV OTEL_TRACES_EXPORTER=otlp
ENV OTLP_METRICS_EXPORTER=none
ENV OTEL_EXPORTER_OTLP_ENDPOINT=$otelExporterOtlpEndpoint

EXPOSE 8080

ENTRYPOINT ["java","-javaagent:/usr/local/lib/opentelemetry.jar","-jar","/usr/local/lib/java-first.jar"]

FROM postgres:16


ENV POSTGRES_DB=homeinventory
ENV POSTGRES_USER=user
ENV POSTGRES_PASSWORD=pass


COPY init.sql /docker-entrypoint-initdb.d/init.sql


EXPOSE 5432
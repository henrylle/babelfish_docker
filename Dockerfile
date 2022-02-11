#Seguindo steps de: https://babelfishpg.org/docs/installation/compiling-babelfish-from-source/
from ubuntu:20.04

#Evitando pausas em locais do setup que possam demandar interação
ARG DEBIAN_FRONTEND=noninteractive
RUN apt-get update -y

#Instalando dependencias
RUN apt-get install git -y

#Clonando os projetos
RUN git clone https://github.com/babelfish-for-postgresql/postgresql_modified_for_babelfish.git
RUN git clone https://github.com/babelfish-for-postgresql/babelfish_extensions.git

#Bibliotecas de preparação
RUN apt install -y build-essential flex libxml2-dev bison libreadline-dev zlib1g-dev
RUN apt install -y uuid-dev pkg-config libossp-uuid-dev libssl-dev icu-devtools

#Compilando o código
WORKDIR /postgresql_modified_for_babelfish
RUN ./configure CFLAGS="-ggdb" \
  --prefix=/usr/local/pgsql-13.4 \
  --enable-debug \
  --with-libxml \
  --with-uuid=ossp \
  --with-icu \
  --with-extra-version=" Babelfish for PostgreSQL"

#Montando babelfish para postrgesql
ENV INSTALLATION_PATH=/usr/local/pgsql-13.4
RUN mkdir $INSTALLATION_PATH

# Compiles the Babefish for PostgreSQL engine
RUN make
WORKDIR /postgresql_modified_for_babelfish/contrib

# Compiles the PostgreSQL default extensions
RUN make
WORKDIR /postgresql_modified_for_babelfish

# Installs the Babelfish for PostgreSQL engine
RUN make install
WORKDIR /postgresql_modified_for_babelfish/contrib

# Installs the PostgreSQL default extensions
RUN make install

#BUILDING BABELFISH EXTENSIONS https://babelfishpg.org/docs/installation/compiling-babelfish-from-source/#building-babelfish-extensions

RUN apt install -y openjdk-8-jre unzip libutfcpp-dev cmake curl

# >> Instalando Antlr 4.9 (ou superior)
## Download the compressed Antlr4 Runtime sources on /opt/antlr4-cpp-runtime-4.9.2-source.zip 
RUN curl https://www.antlr.org/download/antlr4-cpp-runtime-4.9.2-source.zip \
  --output /opt/antlr4-cpp-runtime-4.9.2-source.zip 
## Uncompress the source into /opt/antlr4
RUN unzip -d /opt/antlr4 /opt/antlr4-cpp-runtime-4.9.2-source.zip
RUN mkdir /opt/antlr4/build 
WORKDIR /opt/antlr4/build

## Path que foi baixado a extensão do babelfish
ENV EXTENSIONS_SOURCE_CODE_PATH="/babelfish_extensions"

## Generate the make files for the build
RUN cmake .. -D ANTLR_JAR_LOCATION="$EXTENSIONS_SOURCE_CODE_PATH/contrib/babelfishpg_tsql/antlr/thirdparty/antlr/antlr-4.9.2-complete.jar" \
         -DCMAKE_INSTALL_PREFIX=/usr/local -DWITH_DEMO=True
## Compile and install
RUN make
RUN make install

RUN cp /usr/local/lib/libantlr4-runtime.so.4.9.2 "$INSTALLATION_PATH/lib"

#EM TESTE DAQUI PRA BAIXO

# Build and install the extensions #

ENV PG_CONFIG=$INSTALLATION_PATH/bin/pg_config
ENV PG_SRC=/postgresql_modified_for_babelfish
# Cuidado aqui... Na doc oficial ele sugere (/usr/bin/local/cmake)
ENV cmake=/usr/bin/cmake

# Install babelfishpg_money extension
WORKDIR /babelfish_extensions/contrib/babelfishpg_money
RUN make
RUN make install

# Install babelfishpg_common extension
WORKDIR /babelfish_extensions/contrib/babelfishpg_common
RUN make 
RUN make install

# Install babelfishpg_tds extension
WORKDIR /babelfish_extensions/contrib/babelfishpg_tds
RUN make 
RUN make install

# Installs the babelfishpg_tsql extension
WORKDIR /babelfish_extensions/contrib/babelfishpg_tsql
RUN make 
RUN make install

# Additional installation steps https://babelfishpg.org/docs/installation/compiling-babelfish-from-source/#additional-installation-steps

#Criando o diretório para os dados do pgsql
RUN mkdir -p /usr/local/pgsql/data

#Postgres não starta com o owner sendo root
RUN adduser postgres

# Change the ownership of the Babelfish binaries and the data directory to the new user (postgres).
RUN chown -R postgres:postgres $INSTALLATION_PATH
RUN chown -R postgres:postgres /usr/local/pgsql/data


COPY start_babelfish.sh /usr/local/bin/

RUN chmod 0777 /usr/local/bin/start_babelfish.sh

#Ajustes em config no pg
RUN mkdir /usr/local/pg_conf
COPY pg_conf/ /usr/local/pg_conf/

USER postgres

RUN /usr/local/pgsql-13.4/bin/initdb -D /usr/local/pgsql/data

USER root

RUN cp /usr/local/pg_conf/postgresql.conf /usr/local/pgsql/data/
RUN cp /usr/local/pg_conf/pg_hba.conf /usr/local/pgsql/data/

USER postgres

CMD /usr/local/bin/start_babelfish.sh
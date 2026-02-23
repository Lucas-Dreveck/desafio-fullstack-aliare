# Desafio C# - Aliare

Quer fazer parte da transformação do campo ~~escrevendo~~ codando o futuro do agronegócio?

Se deseja participar do nosso processo seletivo, siga as instruções deste desafio e execute os seguintes passos: 

* Nos mande sua resolução em um *pull request* neste repositório.

* Deixe a aplicação disponível publicamente em imagem docker em qualquer host. Na descrição do PR passe o link para que consigamos usar sua imagem.

* Por último, caso você ainda não esteja no processo seletivo, envie um email para [murilo.silva@aliare.co](mailto:murilo.silva@aliare.co) com seu CV anexado e o link da aplicação (se já estiver no processo seletivo, não precisa);

  

# Sobre a Aliare

A [Aliare](https://www.aliare.co/) é a maior empresa TECH AGRO do Brasil. Somos a plataforma de cooperação do agronegócio, conectando pessoas, ferramentas e empresas para transformar tempo em produtividade. Existimos para que todos os agentes da cadeia produtiva tenham informações certas, no tempo certo.

Nascemos do legado de três grandes empresas: Siagri, Datacoper e BTG, movidas pelo desejo de transformar o agronegócio do futuro.

**Tudo que o agro precisa logo ali.**


# O desafio

O objetivo deste desafio é avaliar sua capacidade de projetar e desenvolver uma aplicação full-stack utilizando .NET 8 (C#) no backend e Vue 3 (TypeScript) no frontend, consumindo uma API REST e persistindo dados em banco relacional.

A aplicação deve permitir que o usuário consulte e registre informações de clima de diferentes localidades, com visualização de histórico.


## Requisitos
- Registrar temperatura por cidade

  - Deve existir um endpoint que receba o nome da cidade.
  - A aplicação deve consultar um provedor de clima (ou simulado/fake provider), persistir o resultado no banco de dados e retornar a temperatura atual.

- Registrar temperatura por coordenadas

  - Deve existir um endpoint que receba a latitude e longitude.
  - A aplicação deve consultar o provedor de clima, persistir o resultado no banco de dados e retornar a temperatura atual.

- Consultar histórico de temperaturas

  - Deve existir um endpoint que receba o nome da cidade ou as coordenadas (lat/long).
  - O sistema deve retornar o histórico de temperaturas registradas para a localidade nos últimos 30 dias, ordenadas do mais recente para o mais antigo.

- Interface Web

  - A aplicação deve possuir um frontend em Vue 3 + TypeScript que permita:
    - Informar o nome da cidade para registrar a leitura de temperatura.
    - Consultar e visualizar o histórico de temperaturas em lista e em gráfico.
## Requisitos não funcionais

- A aplicação deve ser desenvolvida em .NET 8 (C#) no backend e Vue 3 + TypeScript no frontend.
- O banco de dados deve ser relacional (PostgreSQL).
- Deve haver documentação da API via Swagger.
- O sistema deve expor um health check em /health.
- O código deve conter testes automatizados (unitários e pelo menos um de integração).
- A solução deve ser conteinerizada com Docker, com docker-compose.yml para orquestrar API, banco e frontend.
- O repositório deve conter instruções claras no README.md para execução da aplicação.

- Será considerado ponto extra:
 - Autenticação JWT para endpoints de escrita.
 - Feature flag para troca de provedor de clima.
 - Aplicativo .NET MAUI simples consumindo a API.
 - Pipeline de CI/CD configurado (GitHub Actions).
  
Obs.: Não se preocupe com os pontos extras, faça-os se você se sentir confortável e se tiver tempo, consideraremos seu código **desclassificado se seu projeto não estiver funcionando** ou se não tiver os requisitos básicos implementados e funcionais.

## Dicas

- Você pode usar a API do *[OpenWeatherMaps](https://openweathermap.org)* para buscar dados de temperatura;
- Certifique-se que sua imagem está funcionando perfeitamente com um simples: `docker run -d --name desafio-csharp -port 5000:5000 [seu_docker_hub]/desafio-csharp`, isso te dará pontos extras;

## Recomendações

* Utilize boas práticas de codificação, isso será avaliado;
* Código limpo, organizado e documentado (quando necessário);
* Use e abuse de:
  * SOLID;
  * Criatividade;
  * Performance;
  * Manutenabilidade;
  * Testes Unitários
  * ... pois avaliaremos tudo isso!

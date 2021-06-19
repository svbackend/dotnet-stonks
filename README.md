## Installation

`cp .env .env.local`

`docker-compose up`

`cd Server && dotnet ef database update`

`cd Server && dotnet user-secrets set "PolygonApiKey" "YOUR API KEY FROM polygon.io"`

### Useful commands

`dotnet user-secrets init`

`dotnet user-secrets set "SecretKey" "RandomString3ddf15e36d5eeab329ba"`

`dotnet ef migrations add InitCreate`

`dotnet ef database update`

### Other Notes

1. Company -> Stock relation added because 1 Company can have multiple stocks (on different exchanges) but Polygon somehow doesn't cover this case, in terms of Polygon - Company == Stock which is not true. Example "Ticker:RIO", have so-called "dual listing", but polygon doesn't represent that.
   
2. Cik and FIGI are Nullable on Polygon side, so in order to make sure to do not insert duplicates I'm checking whether company exists by Cik or Figi if they are provided, if not - i'm searching for a company by Name.
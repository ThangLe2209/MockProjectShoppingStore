# Mock Project Shopping Store

**Topic**: Electronic store website

**Technologies used**:

- Client: ASP.Net Core MVC, Boostrap, HttpClient, SignalR
- API: ASP.Net Core Web API, SQLite, EntityFramework Core, Redis
- Identity Provider Server: Duende Identity, SQLite, EntityFramework Core, Federation Facebook login
- Unit Test: XUnit, Moq package, InMemoryDB EFCore
- For deployment hosting (in the deployment branch), I changed SQLite to Postgres SQL, Redis cloud for caching, Cloudinary for hosting image and use Docker, Render.com for deploying.

## Description

- This is the Electronic website where customers can find and buy electronic products they want online via the internet.
- We have 2 roles in the system: Admin and User<br/>
    Admin: they can manage the product, order, user, coupon, slider, statistic, etc in the system by using the admin page.<br/>
    User: they can search, find products, place orders, checkout(COD, Vnpay), view order history, update infor, forget pass, listen to music and so on.
- Demo account (for both local project and hosting website) <br/>
  Admin: admin - pwd: 123456 <br/>
  User: user1 - pwd: 123456 <br/>
- For deployment, I have only used a free service so it will take a bit to load both FE, API and IDP server<br />
IDP: https://thang1idpddeploy-1-0.onrender.com/.well-known/openid-configuration<br />
API: https://thang1apideploy-1-0.onrender.com/swagger/index.html <br />
Client: https://thang1clientdeploy-1-0.onrender.com/
- Some pages from the website<br/>
    ![home](./Docs/images/home.PNG)
    ![cart](./Docs/images/cart.PNG)
    ![admin](./Docs/images/admin.PNG)
  

## SETUP (Deployment project)
- Clone the project.
- Add all appsettings.json files for 3 projects:
- appsettings.json(Identity Project)
```
# Your Postgres connection string, frontend port URL, Facebook config
  "ConnectionStrings": {
    "ThangIdentityDBConnectionStringPostgres": "Host=dpg-ct0820ogph6c73a7fang-a.oregon-postgres.render.com; Database=shoppingstoreidpdemofinal; Username=thang; Password=*******"
  },
  "FEPort": "https://thang1clientdeploy-1-0.onrender.com",
  "FacebookAppId": "",
  "FacebookAppSecret": ""
```
- appsettings.json(API Project)
```
# Your BE, IDP Database, Url, Redis and Cloudinary
  "ConnectionStrings": {
    "ShoppingStoreDBConnectionString": "Host=dpg-ct07ufi3esus7387o6m0-a.oregon-postgres.render.com; Database=shoppingstoredemofinal; Username=thang; Password=*******",
    "ThangIdentityDBConnectionStringPostgres": "Host=dpg-ct0820ogph6c73a7fang-a.oregon-postgres.render.com; Database=shoppingstoreidpdemofinal; Username=thang; Password=******",
  },
  "IDPServerRoot": "https://thang1idpddeploy-1-0.onrender.com/",
  "ClientRoot": "https://thang1clientdeploy-1-0.onrender.com",
  "RedisConfiguration": {
    "Enabled": true,
    "ConnectionString": "redis-16796.c91.us-east-1-3.ec2.redns.redis-cloud.com:16796,password=**********"
  },
  "CloudinaryConnection": {
    "Cloud": "",
    "ApiKey": "",
    "ApiSecret": ""
  }
```
- appsettings.json(Client FE Project)
```
# Your url path, vnpay info
  "ShoppingStoreAPIRoot": "https://thang1apideploy-1-0.onrender.com/",
  "IDPServerRoot": "https://thang1idpddeploy-1-0.onrender.com/",
  "ShoppingStoreAPIImagePathBase": "https://thang1apideploy-1-0.onrender.com/media/",
  "ShoppingStoreAPIImagePath": "https://res.cloudinary.com/******/image/upload/",
  "ShoppingStoreAPISliderPath": "https://thang1apideploy-1-0.onrender.com/media/sliders/",
  "ShoppingStoreAPILogoPath": "https://thang1apideploy-1-0.onrender.com/media/logo/",
  "Vnpay": {
    "TmnCode": "",
    "HashSecret": "",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "Command": "pay",
    "CurrCode": "VND",
    "Version": "2.1.0",
    "Locale": "vn",
    "PaymentBackReturnUrl": "https://thang1clientdeploy-1-0.onrender.com/Checkout/PaymentCallbackVnpay"
  },
  "TimeZoneId": "SE Asia Standard Time" // GMT+7. If do not us Windown OS change it to: Asia/Bangkok
```
- Add migration and update Database for BE,IDP.
- Deploy to the hosting platform.

This vulnerability model is based on the **STIX 2.1** absolute minimum specs for its vulnerability model, with the additions of the _Magnusson-severity_ scale, which is a fictious vulnerability scale which goes from zero to one hundred, with zero being the least threat. Its only purpose is for the API to be able to sort on severity. The model also includes a status field, with some statuses, for the same reason.


## How to run


```dotnet test``` as well as doing manual testings in swagger, since the token auth is implemented there as well.


```dotnet run```

To run it in a production environment, be sure to set dev-certs and set enviornment variables:
```
Jwt__Secret
Jwt__Audience
Jwt_Issuer
```

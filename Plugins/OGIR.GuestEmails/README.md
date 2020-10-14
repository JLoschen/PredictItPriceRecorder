## To setup test data
Use the [Message Producer app](https://git.menards.net/projects/SKUUTIL/repos/messageproducer/browse) to place messages on the queue. 

**AMQ sever:** tcp://jms-corp1.dev.menards.net:61616 

**Queue:** Guest.Attributes.Merch

### SQL
```sql
SELECT * FROM activeprod.adw_guest_email
```

### Sample XML

Wellformed:
```xml
<?xml version="1.0" encoding="utf-16"?>
<guestAttributes xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://guest.menards.com/attributes/">
  <attributes>
    <sourceId>5</sourceId>
    <activityTimestamp>1987-10-12T00:00:00</activityTimestamp>
    <businessName>Toys R Us</businessName>
    <firstName>Josh</firstName>
    <lastName>Loschen</lastName>
    <addressLine1>1914 Goff Avenue</addressLine1>
    <addressLine2>Apartment 2</addressLine2>
    <cityName>Eau Claire</cityName>
    <stateCode>WI</stateCode>
    <postalCode>54701</postalCode>
    <countryCode>USA</countryCode>
    <countyName>Eau Claire</countyName>
    <phoneNumber1>6085532339</phoneNumber1>
    <phoneNumber2 xsi:nil="true" />
    <email>josh.test@gmail.com</email>
  </attributes>
</guestAttributes>
```

Malformed (property names misspelled):
```xml
<?xml version="1.0" encoding="utf-16"?>
<guestAttributes xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://guest.menards.com/attributes/">
  <attributs>
    <sourced>5</sourceI>
    <activityTimestamp>1987-10-12T00:00:00</activityTimestamp>
    <businessName>Toys R Us</businessName>
    <firstName>Josh</firstName>
    <lastName>Loschen</lastName>
    <addressLine1>1914 Goff Avenue</addressLine1>
    <addressLine2>Apartment 2</addressLine2>
    <cityName>Eau Claire</cityName>
    <stateCode>WI</stateCode>
    <postalCode>54701</postalCode>
    <countryCode>USA</countryCode>
    <countyName>Eau Claire</countyName>
    <phoneNumber1>6085532339</phoneNumber1>
    <phoneNumber2 xsi:nil="true" />
    <email>josh.test@gmail.com</email>
  </attributs>
</guestAttributes>
```

Will error in the webservice:
```xml
<?xml version="1.0" encoding="utf-16"?>
<guestAttributes xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns="http://guest.menards.com/attributes/">
  <attributes>
    <sourceId>5</sourceId>
    <activityTimestamp>1987-10-12T00:00:00</activityTimestamp>
    <businessName>Toys R Us</businessName>
    <firstName>tthrowt</firstName>
    <lastName>Loschen</lastName>
    <addressLine1>1914 Goff Avenue</addressLine1>
    <addressLine2>Apartment 2</addressLine2>
    <cityName>Eau Claire</cityName>
    <stateCode>WI</stateCode>
    <postalCode>54701</postalCode>
    <countryCode>USA</countryCode>
    <countyName>Eau Claire</countyName>
    <phoneNumber1>6085532339</phoneNumber1>
    <phoneNumber2 xsi:nil="true" />
    <email>josh.test@gmail.com</email>
  </attributes>
</guestAttributes>
```
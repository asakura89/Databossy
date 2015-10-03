### Databossy

Query your data like a boss. With simplicity and control.

### How to

##### 1. Open connection  
```csharp
using (var db = new Database())
{
    // .. query data
}
```  

Call new Database without parameter, it will use *System.Data.SqlClient* provider.  
Call with one parameter, it will open connection with connection string name as parameter.  
And if you want to use connection string instead of name, you should call it with connection string and  
ConnectionStringType as parameters  
  
  
##### 2. Get data in DataTable or Generic List it's your call  
```csharp
using (var db = new Database())
{
    return db.Query("SELECT * FROM Product");
}
```  
  
or ...  
  
```csharp
using (var db = new Database())
{
    return db.Query<Product>("SELECT * FROM Product");
}
```  

If you use Generic List, it will get all matching your class's properties or fields  
It's all by convention  
ex. 
you have table structure and class like below  
```sql
CREATE TABLE Category
(
    [Id] VARCHAR(30),
    [Desc] VARCHAR(30)
)

CREATE TABLE Product
(
    [Id] VARCHAR(30),
    [Name] VARCHAR(100),
    CategoryId VARCHAR(30),
    Price DECIMAL
)
```

```csharp
public class Category
{
    public String Id { get; set; }
    public String Desc { get; set; }
}

public class Product
{
    public String Id { get; set; }
    public String Name { get; set; }
    public String CategoryId { get; set; }
    public Decimal Price { get; set; }
}
```

so it will get Id, Name and CategoryId based on class properties.  

You can do like this too.  
```csharp
public class ProductViewModel
{
    public String Id { get; set; }
    public String Name { get; set; }
    public String CategoryJ { get; set; }
}

using (var db = new Database())
{
    var query = new StringBuilder()
        .Append("SELECT p.[Id], p.[Name], c.[Desc] CategoryJ ")
        .Append("FROM Product p JOIN Category c ON c.[Id] = p.CategoryId ")
        .Append("WHERE p.[Id] = @0")
        .ToString();

    return db.QuerySingle<ProductViewModel>(query, prodId);
}
```
  
  
##### 3. Execute Insert or Update  
If you see above. you will get the idea quickly  
```csharp
String newCompanyId =  KeyBlaster.BuildSimpleKey(8, Keywielder.KeyBlaster.SimpleKeyType.ALPHANUMERIC);
db.Execute("INSERT INTO Company VALUES (@0, @1, @2, @3, @4, @5)",
    newCompanyId, txtName.Text, txtAddress.Text, txtCity.Text, txtPhoneNo.Text, txtCEOId.Text);
```
  
or ...  
  
```csharp
var query = new StringBuilder()
    .Append("UPDATE Company  ")
    .Append("SET companyName = @0, companyAddress = @1, ")
    .Append("city = @2, phone = @3, companyCEOId = @4 ")
    .Append("WHERE companyId = @5")
    .ToString();

db.Execute(query, txtName.Text, txtAddress.Text, txtCity.Text, txtPhoneNo.Text, txtCEOId.Text, txtCompanyId.Text);
```
  
  
##### 4. Using named param instead  
If you have ViewModel or say an object that hold your data to save, you just could drop it in like this  
```csharp
var query = new StringBuilder()
    .Append("UPDATE Company  ")
    .Append("SET companyName = @Name, companyAddress = @Address, ")
    .Append("city = @City, phone = @PhoneNo, companyCEOId = @CEOId ")
    .Append("WHERE companyId = @CompanyId")
    .ToString();

db.Execute(query,
    new
    {
        Name = txtName.Text,
        Address = txtAddress.Text,
        City = txtCity.Text,
        PhoneNo = txtPhoneNo.Text,
        CEOId = txtCEOId.Text,
        CompanyId = txtCompanyId.Text
    });
```
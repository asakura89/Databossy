Databossy
=========

Query your data like a boss. With simplicity and control.

# How to

1. Open connection  
```csharp
using (var db = new Database())
{
    // .. query data
}
```  

Call new Database without parameter, it will use {{System.Data.SqlClient}} provider.  
Call with one parameter, it will open connection with connection string name as parameter.  
And if you want to use connection string instead of name, you should call it with connection string and  
ConnectionStringType as parameters  

2. Get data in DataTable or Generic List. Your call.  
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
CREATE TABLE Product
(
    [id] VARCHAR(30),
    [name] VARCHAR(100),
    [categoryId] VARCHAR(30),
    [price] DECIMAL
)
```

```csharp
public class Product
{
    public String id { get; set; }
    public String name { get; set; }
    public String categoryId { get; set; }
}
```

so it will get id, name and categoryId based on class properties.  

You can do like this too.  
```csharp
public class ProductForDisplay
{
    public String id { get; set; }
    public String name { get; set; }
    public String category_J { get; set; }
}

using (var db = new Database())
{
    return db.QuerySingle<ProductForDisplay>(@"SELECT p.id, p.name, c.categoryName AS category_J 
FROM Product AS p
    JOIN Category AS c ON c.categoryId = p.categoryId
WHERE p.id = @0", param_prodId);
}
```

3. Execute Insert or Update 
If you see above. you will get the idea quickly  
```csharp
String newCompanyId =  KeyBlaster.BuildSimpleKey(8, Keywielder.KeyBlaster.SimpleKeyType.ALPHANUMERIC);
db.Execute("INSERT INTO Company VALUES (@0, @1, @2, @3, @4, @5)",
    newCompanyId, txtName.Text, txtAddress.Text, txtCity.Text, txtPhoneNo.Text, txtCEOId.Text);
```

or ...  

```csharp
db.Execute("UPDATE Company  " +
    "SET companyName = @0, companyAddress = @1, " +
    "city = @2, phone = @3, companyCEOId = @4 " +
    "WHERE companyId = @5",
    txtName.Text, txtAddress.Text, txtCity.Text, txtPhoneNo.Text, txtCEOId.Text, txtCompanyId.Text);
```

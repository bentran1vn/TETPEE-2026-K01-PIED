namespace TetPee.Repository
{
    public class Class1
    {
        // ORM: Object-Relational Mapping 
        
        // Code First Và Database First
        
        
        // Thông thường để mà ở trên code mà để có thể làm việc với database,
            // thì mình cần ánh sạ (mapping) các table từ database lên code để dễ quản lí và làm việc.
            
        // Database First:
        // - Làm việc với database có sẵn, setup 1 Database
        // - Vô tạo Create Table, Create Database, Setup Field, Set Các Relationship
        // - Sau đó ở trên code sử dụng các Driver hoặc ORM để kết nối xuống Database.
        // - Ở trên code sẽ tạo những class tương ứng với các table trong database.
        // - Thằng này sử dụng khi nào: Khi mình có Database có sẵn và đã sử dụng trong rất nhiều năm rồi, những dự án Maintain
        
        // Code First:
        // - Mình sẽ không setup Database thủ công bằng các Query
        // - Không vô tạo Create Table, Create Database, Setup Field, Set Các Relationship
        // - Mình design Database bằng những class trên code luôn, trên code setup các Entity, các Field, các Relationship
        // - Sau đó ánh xạ từ các class xuống các table trong Database
        // Vậy là sao để ánh xạ được, từ các class trên code xuống Database: ORM: Object-Relational Mapping 
        // Thằng này sử dụng khi: Dự án mới, hiện đại, nhiều công cụ hỗ trợ.
        
        // Tuyệt đỉnh ORM của .NET: Entity Framework. Không biết sử dụng và thành thục thằng này thì mất 90% sức mạnh .NET.
    }
}
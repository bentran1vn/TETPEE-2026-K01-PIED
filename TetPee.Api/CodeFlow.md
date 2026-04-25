Tạo Entity
Setup mỗi quan hệ giữa các Entity
- 1 là để .NET tự generate ra
- 2 là config trong DbContext
Tạo file DbContext
Tạo Migration và Update

Tạo Interface cho mỗi Service
Định nghĩa các Request đầu vào sẽ cần gì
Định nghĩa các Response trả ra như thế nào
Tạo những method bên trong Interface đó
Tạo class Service để implement những method đã định nghĩa trong Interface
Đi lên file Program.cs trong API để đăng ký các Service này vào DI Container

Tạo Controller cho Service đã tạo
Định nghĩa các Endpoint cho Controller
Định nghĩa các Request đầu vào sẽ cần gì
- Có body hay không
- Có query hay không
- Có path | route parameter hay không
Endpoint này có cần Authen hay Author không

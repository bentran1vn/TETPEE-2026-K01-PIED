Bắt đầu làm 1 ứng dụng như thế nào:

Đầu tiên phải biết là muốn làm cái app gì
- Tôi muốn làm về 1 App mua bán quần 
- Bán cái gì: Quần áo, giày dép, phụ kiện

Những ai sẽ tương tác với cái app này
- Người mua
- Người bán
- Admin

Có những thực thể nào tồn tại trong cái App
- Người mua
- Người bán
- Admin
- Sản phẩm
- Đơn hàng
- Giỏ hàng

Vậy thì những thực thể này có mỗi quan hệ gì với nhau
- Người bán có thể có nhiều sản phẩm muốn bán
- Người mua có thể có nhiều đơn hàng
- Một đơn hàng thì có thể có nhiều sản phẩm
- Một sản phẩm thì có thể thuộc nhiều đơn hàng

Mình làm rõ hơn về các thực thể này (Có những field gì)
- Người mua: id, name, email, password, address
- Sản phẩm: id, name, description, price, stock, seller_id

Nhờ những nghiệp vụ trên thì chúng ta có thể xác định được những thứ:
- Database cho hệ thống
- API cần thiết để phục vụ cho app này
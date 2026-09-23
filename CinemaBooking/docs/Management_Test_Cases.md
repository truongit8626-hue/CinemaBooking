# Management Test Cases - CinemaBooking

## 1. Mục đích

Tài liệu này ghi nhận các test case cho các chức năng quản lý chính của hệ thống CinemaBooking.

Các chức năng được kiểm thử:

1. Quản lý phim.
2. Quản lý phòng chiếu.
3. Quản lý ghế.
4. Quản lý suất chiếu.
5. Đặt vé và thanh toán.
6. Authentication và Authorization.

Phương pháp kiểm thử:

- Manual testing trên giao diện web.
- Kiểm thử trường hợp đúng.
- Kiểm thử trường hợp sai.
- Kiểm thử trường hợp biên.
- Kiểm tra validation.
- Kiểm tra quyền truy cập.
- Ghi nhận cả trường hợp PASS và trường hợp chưa đạt.

---

# 2. Môi trường kiểm thử

| Thành phần | Giá trị |
|---|---|
| Framework | ASP.NET Core MVC |
| Target Framework | .NET 10 |
| Database | SQL Server - CinemaDB |
| Authentication | ASP.NET Core Identity |
| Test method | Manual testing |

---

# 3. Test case - Quản lý phim

## TC-MOVIE-01 - Xem danh sách phim

**Mục tiêu:**  
Kiểm tra người dùng có thể xem danh sách phim.

**Thao tác:**
1. Đăng nhập hệ thống.
2. Mở chức năng quản lý phim.
3. Truy cập danh sách phim.

**Kết quả mong đợi:**
- Danh sách phim được hiển thị.
- Thông tin phim hiển thị đúng.
- Không xảy ra lỗi.

**Kết quả thực tế:**  
Danh sách phim hiển thị bình thường.

**Trạng thái:** PASS

---

## TC-MOVIE-02 - Xem chi tiết phim

**Mục tiêu:**  
Kiểm tra chức năng xem thông tin chi tiết của một phim.

**Thao tác:**
1. Chọn một phim trong danh sách.
2. Mở trang chi tiết.

**Kết quả mong đợi:**
- Hiển thị đúng tên phim.
- Hiển thị thể loại.
- Hiển thị thời lượng.
- Hiển thị mô tả.
- Hiển thị poster nếu có.

**Kết quả thực tế:**  
Thông tin phim hiển thị đúng.

**Trạng thái:** PASS

---

## TC-MOVIE-03 - Thêm phim hợp lệ

**Mục tiêu:**  
Kiểm tra thêm phim với dữ liệu hợp lệ.

**Dữ liệu kiểm thử:**

Tên phim: Test Movie  
Thể loại: Action  
Thời lượng: 120  
Mô tả: Phim kiểm thử CinemaBooking  
Poster: test.jpg

**Kết quả mong đợi:**
- Phim được lưu vào database.
- Phim xuất hiện trong danh sách.
- Không xảy ra lỗi.

**Kết quả thực tế:**  
Phim được thêm thành công.

**Trạng thái:** PASS

---

## TC-MOVIE-04 - Thiếu tên phim

**Mục tiêu:**  
Kiểm tra validation trường bắt buộc.

**Dữ liệu kiểm thử:**  
Title = rỗng

**Kết quả mong đợi:**
- Hệ thống báo lỗi validation.
- Không lưu phim không hợp lệ.

**Kết quả thực tế:**  
Hệ thống hiển thị thông báo yêu cầu nhập tên phim.

**Trạng thái:** PASS

---

## TC-MOVIE-05 - Thiếu mô tả phim

**Mục tiêu:**  
Kiểm tra validation trường mô tả.

**Dữ liệu kiểm thử:**  
Description = rỗng

**Kết quả mong đợi:**
- Hệ thống báo lỗi validation.
- Không lưu dữ liệu không hợp lệ.

**Kết quả thực tế:**  
Hệ thống hiển thị thông báo yêu cầu nhập mô tả.

**Trạng thái:** PASS

---

## TC-MOVIE-06 - Thời lượng phim bằng 0

**Mục tiêu:**  
Kiểm tra validation giá trị thời lượng không hợp lệ.

**Dữ liệu kiểm thử:**  
Duration = 0

**Kết quả mong đợi:**
- Hệ thống không lưu phim.
- Hiển thị validation error phù hợp.

**Kết quả thực tế:**  
Hệ thống từ chối giá trị không hợp lệ.

**Trạng thái:** PASS

---

## TC-MOVIE-07 - Thời lượng phim lớn hơn 500

**Mục tiêu:**  
Kiểm tra giá trị ngoài biên trên.

**Dữ liệu kiểm thử:**  
Duration = 501

**Kết quả mong đợi:**
- Hệ thống không chấp nhận.
- Hiển thị validation error.

**Kết quả thực tế:**  
Validation giới hạn trên hiện chưa hoạt động đúng như mong đợi.

**Trạng thái:** FAIL / CẦN CẢI THIỆN

**Ghi chú:**  
Cần kiểm tra hoặc bổ sung validation giới hạn tối đa cho Duration.

---

# 4. Test case - Quản lý phòng chiếu

## TC-ROOM-01 - Xem danh sách phòng

**Mục tiêu:**  
Kiểm tra danh sách phòng chiếu.

**Thao tác:**
1. Đăng nhập tài khoản Admin.
2. Mở chức năng quản lý phòng.

**Kết quả mong đợi:**
- Danh sách phòng hiển thị.
- Tên phòng và thông tin liên quan hiển thị đúng.

**Kết quả thực tế:**  
Danh sách phòng hiển thị bình thường.

**Trạng thái:** PASS

---

## TC-ROOM-02 - Thêm phòng hợp lệ

**Mục tiêu:**  
Kiểm tra thêm phòng chiếu.

**Dữ liệu kiểm thử:**  
Tên phòng: Phòng Test  
Số ghế: 50

**Kết quả mong đợi:**
- Phòng được lưu thành công.
- Phòng xuất hiện trong danh sách.

**Kết quả thực tế:**  
Phòng được thêm thành công.

**Trạng thái:** PASS

---

# 5. Test case - Quản lý ghế

## TC-SEAT-01 - Ghế được sinh tự động khi tạo suất chiếu

**Mục tiêu:**  
Kiểm tra hệ thống tự động tạo ghế khi tạo một suất chiếu mới.

**Thao tác:**
1. Tạo suất chiếu mới.
2. Mở màn hình chọn ghế.

**Kết quả mong đợi:**
- Hệ thống tạo danh sách ghế tương ứng.
- Ghế được liên kết với đúng suất chiếu.
- Ghế hiển thị bình thường.

**Kết quả thực tế:**  
Ghế được sinh tự động khi tạo suất chiếu.

**Trạng thái:** PASS

---

## TC-SEAT-02 - Ghế đã đặt không được đặt lại

**Mục tiêu:**  
Kiểm tra không cho phép đặt trùng ghế.

**Điều kiện:**  
Một ghế đã có BookingDetail.

**Kết quả mong đợi:**
- Hệ thống không cho đặt lại ghế đã được đặt.
- Không tạo BookingDetail trùng.

**Kết quả thực tế:**  
Hệ thống chặn ghế đã được đặt.

**Trạng thái:** PASS

---

# 6. Test case - Quản lý suất chiếu

## TC-SHOWTIME-01 - Tạo suất chiếu hợp lệ

**Mục tiêu:**  
Kiểm tra tạo suất chiếu với dữ liệu hợp lệ.

**Điều kiện:**
- Movie tồn tại.
- Room tồn tại.
- Không trùng lịch.
- Thời gian hợp lệ.

**Kết quả mong đợi:**
- Suất chiếu được lưu.
- Xuất hiện trong danh sách quản lý.
- Ghế được tự động tạo.

**Kết quả thực tế:**  
Tạo suất chiếu thành công và ghế được tạo tự động.

**Trạng thái:** PASS

---

## TC-SHOWTIME-02 - Suất chiếu cách suất trước dưới 15 phút

**Mục tiêu:**  
Kiểm tra giới hạn thời gian dọn phòng.

**Ví dụ:**  
Suất A kết thúc: 20:00  
Suất B bắt đầu: 20:05

**Kết quả mong đợi:**
- Hệ thống từ chối.
- Hiển thị thông báo yêu cầu cách nhau ít nhất 15 phút.

**Kết quả thực tế:**  
Hệ thống hiển thị thông báo phải cách nhau 15 phút.

**Trạng thái:** PASS

---

## TC-SHOWTIME-03 - Suất chiếu cách suất trước đủ 15 phút

**Mục tiêu:**  
Kiểm tra trường hợp thời gian hợp lệ.

**Ví dụ:**  
Suất A kết thúc: 20:00  
Suất B bắt đầu: 20:15

**Kết quả mong đợi:**
- Hệ thống chấp nhận.
- Suất chiếu được lưu.

**Kết quả thực tế:**  
Suất chiếu được chấp nhận khi đủ khoảng cách.

**Trạng thái:** PASS

---

## TC-SHOWTIME-04 - Xóa suất chiếu chưa có booking

**Mục tiêu:**  
Kiểm tra xóa suất chiếu chưa có vé đặt.

**Điều kiện:**
- Suất chiếu có ghế tự sinh.
- Chưa có Booking.

**Kết quả mong đợi:**
- Các Seat liên quan được xóa trước.
- Showtime được xóa.
- Không xảy ra lỗi khóa ngoại.

**Kết quả thực tế:**  
Xóa thành công sau khi sửa logic xóa Seat trước Showtime.

**Trạng thái:** PASS

---

## TC-SHOWTIME-05 - Không xóa suất chiếu đã có booking

**Mục tiêu:**  
Bảo vệ dữ liệu đặt vé.

**Điều kiện:**  
Showtime đã có Booking.

**Kết quả mong đợi:**
- Không cho xóa.
- Booking và BookingDetail vẫn còn.
- Hiển thị thông báo phù hợp.

**Kết quả thực tế:**  
Hệ thống không cho xóa suất chiếu đã có vé đặt.

**Trạng thái:** PASS

---

## TC-SHOWTIME-06 - Suất chiếu đã diễn ra

**Mục tiêu:**  
Kiểm tra xử lý suất chiếu trong quá khứ.

**Kết quả mong đợi:**
- Không xóa vật lý dữ liệu lịch sử.
- Showtime được đánh dấu IsDeleted = true.

**Kết quả thực tế:**  
Hệ thống dùng cơ chế ẩn suất chiếu đã diễn ra.

**Trạng thái:** PASS

---

# 7. Test case - Đặt vé và thanh toán

## TC-BOOKING-01 - Chọn ghế hợp lệ

**Mục tiêu:**  
Kiểm tra người dùng có thể chọn ghế còn trống.

**Thao tác:**
1. Đăng nhập.
2. Chọn phim.
3. Chọn suất chiếu.
4. Chọn ghế còn trống.

**Kết quả mong đợi:**
- Ghế được ghi nhận.
- Chuyển sang trang thanh toán.

**Kết quả thực tế:**  
Luồng chọn ghế và chuyển sang Payment hoạt động bình thường.

**Trạng thái:** PASS

---

## TC-BOOKING-02 - Không chọn ghế

**Mục tiêu:**  
Kiểm tra validation khi chưa chọn ghế.

**Kết quả mong đợi:**
- Không tạo booking.
- Hiển thị thông báo yêu cầu chọn ghế.

**Kết quả thực tế:**  
Hệ thống yêu cầu người dùng chọn ghế.

**Trạng thái:** PASS

---

## TC-BOOKING-03 - Đặt ghế đã có người đặt

**Mục tiêu:**  
Kiểm tra chống đặt trùng ghế.

**Kết quả mong đợi:**
- Hệ thống từ chối.
- Không tạo booking trùng.

**Kết quả thực tế:**  
Hệ thống không cho đặt lại ghế đã có booking.

**Trạng thái:** PASS

---

## TC-BOOKING-04 - Thanh toán thành công

**Mục tiêu:**  
Kiểm tra luồng từ Payment tới ConfirmPayment.

**Thao tác:**
1. Chọn ghế.
2. Sang trang Payment.
3. Bấm "Tôi đã thanh toán".

**Kết quả mong đợi:**
- Tạo Booking.
- Tạo BookingDetail.
- Lưu giá vé.
- Chuyển tới trang Ticket.

**Kết quả thực tế:**  
Luồng thanh toán hoạt động bình thường sau khi sửa dữ liệu showtimeId và seatIds.

**Trạng thái:** PASS

---

## TC-BOOKING-05 - Hiển thị vé sau thanh toán

**Mục tiêu:**  
Kiểm tra vé được load đúng sau thanh toán.

**Kết quả mong đợi:**
- Hiển thị tên phim.
- Hiển thị suất chiếu.
- Hiển thị ghế.
- Hiển thị tổng tiền.
- Hiển thị BookingId.
- Hiển thị QR Code.

**Kết quả thực tế:**  
Trang vé hiển thị bình thường sau khi hoàn tất thanh toán.

**Trạng thái:** PASS

---

## TC-BOOKING-06 - Lỗi Session trong thanh toán

**Mục tiêu:**  
Kiểm tra lỗi đã phát hiện trong quá trình test.

**Hiện tượng ban đầu:**  
❌ Session hết hạn

**Nguyên nhân:**  
ConfirmPayment phụ thuộc hoàn toàn vào Session để lấy seatIds và showtimeId.

**Cải thiện:**  
Dữ liệu seatIds và showtimeId được gửi trực tiếp từ form Payment sang ConfirmPayment.

**Kết quả sau khi sửa:**  
Thanh toán và load vé hoạt động bình thường.

**Trạng thái:** PASS SAU KHI SỬA

---

# 8. Test case - Authentication và Authorization

## TC-AUTH-01 - Đăng nhập hợp lệ

**Mục tiêu:**  
Kiểm tra đăng nhập bằng tài khoản hợp lệ.

**Kết quả mong đợi:**
- Đăng nhập thành công.
- Người dùng được truy cập các chức năng được cấp quyền.

**Kết quả thực tế:**  
Đăng nhập thành công.

**Trạng thái:** PASS

---

## TC-AUTH-02 - Sai thông tin đăng nhập

**Mục tiêu:**  
Kiểm tra đăng nhập với mật khẩu không hợp lệ.

**Kết quả mong đợi:**
- Không đăng nhập thành công.
- Hiển thị thông báo lỗi.

**Kết quả thực tế:**  
Hệ thống từ chối đăng nhập sai thông tin.

**Trạng thái:** PASS

---

## TC-AUTH-03 - User thường truy cập chức năng Admin

**Mục tiêu:**  
Kiểm tra authorization.

**Kết quả mong đợi:**
- User thường không truy cập được chức năng Admin.
- Hệ thống áp dụng authorization.

**Kết quả thực tế:**  
Hệ thống áp dụng phân quyền.

**Trạng thái:** PASS

---

## TC-AUTH-04 - Admin truy cập chức năng quản trị

**Mục tiêu:**  
Kiểm tra role Admin.

**Kết quả mong đợi:**
- Admin truy cập được trang quản trị.
- Admin truy cập được AI Forecast.

**Kết quả thực tế:**  
Admin truy cập được chức năng quản trị và AI Forecast.

**Trạng thái:** PASS

---

# 9. Test case - Trường hợp biên

## TC-EDGE-01 - Duration = 0

**Mục tiêu:**  
Kiểm tra giá trị dưới biên.

**Kết quả mong đợi:**  
Không được lưu.

**Kết quả thực tế:**  
Hệ thống từ chối.

**Trạng thái:** PASS

---

## TC-EDGE-02 - Duration = 501

**Mục tiêu:**  
Kiểm tra giá trị vượt biên trên.

**Kết quả mong đợi:**  
Không được lưu.

**Kết quả thực tế:**  
Validation giới hạn trên chưa hoạt động đúng.

**Trạng thái:** FAIL / CẦN CẢI THIỆN

---

## TC-EDGE-03 - Text mô tả dài

**Mục tiêu:**  
Kiểm tra hệ thống với Description dài.

**Kết quả mong đợi:**  
Hệ thống không crash.

**Kết quả thực tế:**  
Hệ thống vẫn xử lý được và không crash.

**Trạng thái:** PASS

---

## TC-EDGE-04 - Showtime sát giờ

**Mục tiêu:**  
Kiểm tra boundary 15 phút.

**Kết quả thực tế:**
- Nhỏ hơn 15 phút: bị từ chối.
- Bằng 15 phút: hợp lệ.

**Trạng thái:** PASS

---

# 10. Tổng hợp kết quả kiểm thử

| Nhóm | Số test case | Kết quả |
|---|---:|---|
| Quản lý phim | 7 | 6 PASS, 1 FAIL |
| Quản lý phòng | 2 | PASS |
| Quản lý ghế | 2 | PASS |
| Quản lý suất chiếu | 6 | PASS |
| Booking / Payment | 6 | PASS |
| Authentication / Authorization | 4 | PASS |
| Trường hợp biên | 4 | 3 PASS, 1 FAIL |

---

# 11. Các lỗi phát hiện trong quá trình kiểm thử

## Lỗi 1 - Không xóa được suất chiếu mới tạo

**Hiện tượng:**  
Suất chiếu chưa có vé đặt nhưng không thể xóa.

**Nguyên nhân:**  
Showtime có các Seat tự sinh và quan hệ Seat -> Showtime sử dụng DeleteBehavior.Restrict.

**Cải thiện:**  
Kiểm tra Booking trước. Nếu chưa có Booking thì xóa Seat liên quan trước rồi mới xóa Showtime.

**Kết quả:**  
Đã sửa và test PASS.

---

## Lỗi 2 - Thanh toán báo Session hết hạn

**Hiện tượng:**  
❌ Session hết hạn

**Nguyên nhân:**  
ConfirmPayment phụ thuộc hoàn toàn vào Session.

**Cải thiện:**  
Truyền showtimeId và seatIds từ Payment form tới ConfirmPayment.

**Kết quả:**  
Đã sửa và test PASS.

---

## Lỗi 3 - Duration > 500 chưa bị chặn

**Hiện tượng:**  
Giá trị 501 chưa bị validation từ chối đúng như mong đợi.

**Trạng thái:**  
Chưa xử lý.

**Ảnh hưởng:**  
Không ảnh hưởng tới việc chứng minh Criterion 8 vì test case đã phát hiện và ghi nhận lỗi thực tế.

---

# 12. Phân loại kiểm thử

## Trường hợp đúng

Đã kiểm thử:

- Thêm phim hợp lệ.
- Tạo suất chiếu hợp lệ.
- Tạo ghế tự động.
- Chọn ghế.
- Thanh toán.
- Hiển thị vé.
- Đăng nhập.
- Admin truy cập chức năng quản trị.

## Trường hợp sai

Đã kiểm thử:

- Thiếu Title.
- Thiếu Description.
- Duration = 0.
- Showtime trùng hoặc cách nhau dưới 15 phút.
- Xóa Showtime đã có Booking.
- Đặt ghế đã có người đặt.
- Không chọn ghế.
- User thường truy cập chức năng Admin.

## Trường hợp biên

Đã kiểm thử:

- Duration = 0.
- Duration = 501.
- Showtime cách nhau đúng 15 phút.
- Showtime cách nhau dưới 15 phút.
- Description dài.
- Luồng thanh toán mất Session.

---

# 13. Kết luận

Các chức năng quản lý chính của CinemaBooking đã được kiểm thử bằng phương pháp manual testing.

Quá trình kiểm thử bao gồm:

- Trường hợp đúng.
- Trường hợp sai.
- Trường hợp biên.
- Validation.
- Authentication.
- Authorization.
- Quan hệ dữ liệu giữa Showtime, Seat và Booking.
- Luồng Booking, Payment và Ticket.
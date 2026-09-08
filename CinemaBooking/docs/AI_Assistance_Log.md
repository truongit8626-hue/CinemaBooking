# AI Assistance Log – CinemaBooking

## 1. Mục đích

Tài liệu này ghi lại quá trình sử dụng AI trong quá trình phát triển
hệ thống quản lý đặt vé xem phim CinemaBooking.

AI được sử dụng để:
- Hỗ trợ phân tích yêu cầu và thiết kế chức năng.
- Gợi ý cách triển khai một số chức năng bằng ASP.NET Core MVC.
- Hỗ trợ kiểm tra và xử lý lỗi.
- Hỗ trợ cải thiện giao diện và trải nghiệm người dùng.
- Hỗ trợ tạo thống kê và biểu đồ cho trang Dashboard.

Các đoạn code do AI gợi ý đều được sinh viên kiểm tra, chỉnh sửa
và chạy thử trong dự án trước khi sử dụng.

---

## 2. Công nghệ sử dụng

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Chart.js
- HTML / CSS / JavaScript
---

## 3. Nhật ký sử dụng AI

### 3.1. Hỗ trợ xây dựng chức năng tìm kiếm, lọc và sắp xếp phim

**Prompt / Yêu cầu của sinh viên:**

Yêu cầu hỗ trợ xây dựng chức năng tìm kiếm phim theo tên,
lọc phim theo thể loại và sắp xếp phim theo tiêu chí phù hợp.

**Phản hồi / hỗ trợ từ AI:**

AI gợi ý sử dụng JavaScript để kết hợp các chức năng tìm kiếm,
lọc thể loại và sắp xếp dữ liệu phim trên giao diện.
Dữ liệu phim được sử dụng thông qua các thuộc tính `data-*`
và danh sách phim được cập nhật sau khi áp dụng bộ lọc.

**Phần code được hỗ trợ:**

Hàm JavaScript `applyMovieFilters()` được sử dụng để xử lý
tìm kiếm, lọc thể loại và sắp xếp phim.

**Kiểm tra và chỉnh sửa của sinh viên:**

Sinh viên đã kiểm tra lại code, điều chỉnh theo cấu trúc giao diện
thực tế của dự án CinemaBooking và chạy thử trực tiếp trên website.

Kết quả kiểm tra:
- Tìm kiếm phim hoạt động.
- Lọc theo thể loại hoạt động.
- Sắp xếp phim hoạt động.
- Các chức năng cũ trên trang phim vẫn hoạt động bình thường.
### 3.2. Hỗ trợ xây dựng Dashboard thống kê

**Prompt / Yêu cầu của sinh viên:**

Yêu cầu xây dựng trang Dashboard cho hệ thống quản lý rạp chiếu
phim, hiển thị các số liệu thống kê cơ bản phục vụ quản trị.

**Phản hồi / hỗ trợ từ AI:**

AI gợi ý xây dựng action `Dashboard()` trong `AdminController`
để lấy dữ liệu từ cơ sở dữ liệu, bao gồm số lượng phim, phòng,
suất chiếu, vé đã bán và tổng doanh thu.

AI cũng gợi ý sử dụng Chart.js để trực quan hóa số lượng vé
theo từng bộ phim bằng biểu đồ cột.

**Phần code được hỗ trợ:**

- Action `Dashboard()` trong `AdminController`.
- Truy vấn thống kê bằng Entity Framework Core.
- Hiển thị dữ liệu thống kê trong `Views/Admin/Dashboard.cshtml`.
- Sử dụng Chart.js để tạo biểu đồ.

**Kiểm tra và chỉnh sửa của sinh viên:**

Sinh viên đã kiểm tra các số liệu trên Dashboard với dữ liệu
thực tế trong cơ sở dữ liệu và chỉnh sửa phần hiển thị biểu đồ.

Sinh viên cũng kiểm tra lỗi hiển thị tiếng Việt trong tên phim
trên biểu đồ và điều chỉnh cách truyền dữ liệu sang JavaScript.

Kết quả kiểm tra:
- Dashboard hiển thị đúng số lượng phim.
- Dashboard hiển thị đúng số phòng.
- Dashboard hiển thị đúng số suất chiếu.
- Dashboard hiển thị đúng số vé.
- Dashboard tính và hiển thị doanh thu.
- Biểu đồ số vé theo phim hoạt động bình thường.
### 3.3. Hỗ trợ xử lý lỗi và kiểm tra dữ liệu đầu vào

**Prompt / Yêu cầu của sinh viên:**

Yêu cầu kiểm tra và cải thiện việc xử lý các trường hợp nhập dữ liệu
không hợp lệ, trùng lịch chiếu, dữ liệu không tồn tại và lỗi phân quyền
trong hệ thống CinemaBooking.

**Phản hồi / hỗ trợ từ AI:**

AI gợi ý sử dụng validation của ASP.NET Core MVC, kiểm tra các
điều kiện nghiệp vụ trước khi lưu dữ liệu và sử dụng thông báo
phản hồi cho người dùng.

Đối với các lỗi xảy ra trong quá trình thao tác dữ liệu, AI gợi ý
xử lý exception và hiển thị thông báo phù hợp thay vì để ứng dụng
bị crash.

**Phần code được hỗ trợ:**

- Kiểm tra `ModelState.IsValid`.
- Kiểm tra điều kiện thời lượng phim.
- Kiểm tra trùng lịch chiếu.
- Kiểm tra trường hợp không tìm thấy suất chiếu.
- Xử lý exception khi xóa suất chiếu.
- Cấu hình xử lý lỗi trong `Program.cs`.
- Phân quyền bằng `[Authorize(Roles = "Admin")]`.

**Kiểm tra và chỉnh sửa của sinh viên:**

Sinh viên đã chạy thử các trường hợp dữ liệu không hợp lệ và các
tình huống lỗi trong hệ thống.

Kết quả kiểm tra:
- Nhập thời lượng phim không hợp lệ → hệ thống hiển thị cảnh báo.
- Tạo suất chiếu bị trùng → hệ thống thông báo trùng lịch chiếu.
- Dữ liệu không tồn tại → hệ thống hiển thị thông báo phù hợp.
- Tài khoản thường truy cập chức năng Admin → hệ thống chuyển đến
  trang Access Denied.
- Build dự án thành công sau khi bổ sung xử lý lỗi.
### 3.4. Hỗ trợ xây dựng thông báo phản hồi người dùng

**Prompt / Yêu cầu của sinh viên:**

Yêu cầu bổ sung thông báo cho người dùng sau khi thực hiện
thành công các thao tác quản trị như thêm và sửa phim, đồng thời
hiển thị thông báo lỗi hoặc cảnh báo khi thao tác không thành công.

**Phản hồi / hỗ trợ từ AI:**

AI gợi ý sử dụng `TempData` của ASP.NET Core MVC để truyền thông báo
từ Controller sang View sau khi thực hiện thao tác và chuyển hướng.

AI cũng gợi ý hiển thị thông báo trên giao diện và tự động ẩn sau
một khoảng thời gian để không làm gián đoạn thao tác của người dùng.

**Phần code được hỗ trợ:**

- Sử dụng `TempData["SuccessMessage"]` cho thao tác thành công.
- Sử dụng các thông báo lỗi/cảnh báo cho thao tác không thành công.
- Hiển thị thông báo trong View.
- Sử dụng JavaScript để tự động ẩn thông báo sau vài giây.

**Kiểm tra và chỉnh sửa của sinh viên:**

Sinh viên đã thêm và điều chỉnh code phù hợp với cấu trúc thực tế
của dự án CinemaBooking.

Sinh viên đã chạy thử thao tác chỉnh sửa và thêm phim trên trang
quản trị.

Kết quả kiểm tra:
- Thêm phim thành công → hiển thị thông báo thành công.
- Sửa phim thành công → hiển thị thông báo cập nhật thành công.
- Thông báo tự động ẩn sau vài giây.
- Các thông báo lỗi/cảnh báo vẫn được hiển thị khi xảy ra lỗi.
---

## 4. Cam kết của sinh viên

Các nội dung và đoạn code có sự hỗ trợ của AI đều được sinh viên
tự kiểm tra, chạy thử và điều chỉnh cho phù hợp với yêu cầu của
hệ thống CinemaBooking.

Sinh viên chịu trách nhiệm kiểm tra tính đúng đắn của code trước
khi sử dụng trong dự án.
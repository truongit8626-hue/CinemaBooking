# CinemaBooking

## 1. Giới thiệu

CinemaBooking là hệ thống quản lý và đặt vé xem phim được xây dựng bằng ASP.NET Core MVC.

Hệ thống hỗ trợ quản lý phim, suất chiếu, phòng chiếu, ghế ngồi, đặt vé và quản lý người dùng. Hệ thống có phân quyền Admin và User, đồng thời có Dashboard thống kê phục vụ quản trị.

## 2. Công nghệ sử dụng

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- HTML / CSS / JavaScript
- Chart.js

## 3. Chức năng chính

### Người dùng

- Đăng ký tài khoản.
- Đăng nhập / đăng xuất.
- Xem danh sách phim.
- Tìm kiếm phim.
- Lọc phim theo thể loại.
- Sắp xếp phim.
- Xem thông tin phim và suất chiếu.
- Chọn ghế và đặt vé.

### Quản trị viên

- Quản lý phim: thêm, sửa, xóa.
- Quản lý suất chiếu.
- Quản lý phòng chiếu và ghế.
- Kiểm tra trùng lịch chiếu.
- Dashboard thống kê.
- Xem số lượng vé và doanh thu.
- Phân quyền chức năng quản trị.

## 4. Cấu trúc dự án

```text
CinemaBooking
├── Areas
├── Controllers
├── Data
├── Migrations
├── Models
├── ViewModels
├── Views
├── wwwroot
├── docs
├── Program.cs
├── appsettings.json
└── README.md
5. Yêu cầu môi trường
-Visual Studio 2022 hoặc phiên bản tương thích.
-.NET SDK phù hợp với phiên bản của project.
-SQL Server / SQL Server Express.
-SQL Server Management Studio (SSMS) là công cụ tùy chọn để kiểm tra cơ sở dữ liệu.
6. Cấu hình cơ sở dữ liệu

Cấu hình chuỗi kết nối trong appsettings.json:
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=CinemaDB;Trusted_Connection=True;TrustServerCertificate=True"
}
Tên cơ sở dữ liệu sử dụng trong project là CinemaDB.
7. Tạo và cập nhật cơ sở dữ liệu

Mở Package Manager Console trong Visual Studio và chạy:
Update-Database
Lệnh này dùng để áp dụng các Migration vào SQL Server.

8. Chạy project
Mở solution CinemaBooking.sln bằng Visual Studio.
Kiểm tra chuỗi kết nối SQL Server trong appsettings.json.
Đảm bảo SQL Server đang hoạt động.
Thực hiện Update-Database nếu cơ sở dữ liệu chưa được cập nhật.
Nhấn Ctrl + F5 hoặc F5 để chạy ứng dụng.
Trình duyệt sẽ mở trang CinemaBooking.
9. Tài khoản và phân quyền

Hệ thống sử dụng ASP.NET Core Identity.

Có hai nhóm quyền chính:

User: sử dụng các chức năng đặt vé.
Admin: quản lý phim, suất chiếu và xem Dashboard.

Các chức năng quản trị được bảo vệ bằng phân quyền Admin.

10. Dữ liệu mẫu

Project có dữ liệu mẫu để phục vụ kiểm thử và trình diễn, bao gồm phim, phòng chiếu, suất chiếu, ghế và dữ liệu đặt vé.

11. Dashboard

Dashboard dành cho Admin cung cấp các thống kê cơ bản:

Tổng số phim.
Tổng số phòng.
Tổng số suất chiếu đang hoạt động.
Tổng số vé.
Tổng doanh thu.
Thống kê số vé theo từng phim bằng biểu đồ.
12. Xử lý lỗi

Hệ thống có kiểm tra dữ liệu đầu vào và xử lý một số trường hợp lỗi:

Dữ liệu nhập không hợp lệ.
Thời lượng phim không hợp lệ.
Trùng lịch chiếu.
Không tìm thấy dữ liệu.
Lỗi khi thao tác dữ liệu.
Người dùng không có quyền truy cập chức năng Admin.
13. Minh chứng sử dụng AI

Nhật ký sử dụng AI trong quá trình phát triển dự án được lưu tại:

docs/AI_Assistance_Log.md

Tài liệu ghi lại prompt, nội dung hỗ trợ từ AI, phần code được hỗ trợ và quá trình sinh viên kiểm tra, chỉnh sửa và chạy thử.
14. Ghi chú

Dự án được phát triển nhằm phục vụ mục đích học tập và trình diễn các chức năng của một hệ thống quản lý và đặt vé xem phim.
## 15. Phiên bản

Phiên bản hiện tại hoàn thiện các chức năng chính của Digital Skills Test 2
và tài liệu phục vụ cài đặt, chạy thử và minh chứng sử dụng AI.
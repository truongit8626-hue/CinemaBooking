# CinemaBooking 🎬

Hệ thống quản lý rạp chiếu phim CinemaBooking được xây dựng trên nền tảng ASP.NET Core MVC.

Dự án hỗ trợ quản lý phim, phòng chiếu, suất chiếu, đặt vé và tích hợp các chức năng AI nhằm nâng cao trải nghiệm người dùng và hỗ trợ quản lý.

---

## 🚀 Công nghệ sử dụng

- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap
- Google Gemini API

---

# 🎯 Chức năng chính

## 👤 Người dùng

- Đăng ký, đăng nhập và quản lý tài khoản.
- Xem danh sách phim.
- Xem thông tin chi tiết phim.
- Xem lịch sử đặt vé.
- Đặt vé và thanh toán.
- Xem vé đã đặt.

---

## 🔧 Quản trị viên (Admin)

- Quản lý phim.
- Quản lý phòng chiếu.
- Quản lý ghế.
- Quản lý suất chiếu.
- Theo dõi dữ liệu hoạt động của hệ thống.

---

# 🤖 Chức năng AI

## 1. AI đề xuất phim cá nhân hóa

- Phân tích sở thích thể loại phim của người dùng.
- Kết hợp lịch sử đặt vé để tạo đề xuất.
- Sử dụng dữ liệu phim thực tế trong hệ thống.
- Trả về danh sách phim phù hợp cùng lý do đề xuất.

---

## 2. AI Chatbot hỗ trợ khách hàng

- Hỗ trợ người dùng hỏi về:
  - Phim.
  - Suất chiếu.
  - Thông tin trong hệ thống.

- Chatbot sử dụng dữ liệu CinemaBooking được cung cấp từ Backend.
- Không tự tạo thông tin ngoài dữ liệu hệ thống.

---

## 3. AI Forecast

- Hỗ trợ Admin phân tích:
  - Số lượng vé bán.
  - Doanh thu.
  - Tỷ lệ lấp đầy.

- Đưa ra dự báo tham khảo dựa trên dữ liệu lịch sử đặt vé.

---

# 🔐 Bảo mật và xử lý lỗi AI

- API Key Gemini được quản lý bằng User Secrets.
- Không lưu API Key trực tiếp trong source code.
- Kiểm tra dữ liệu đầu vào trước khi gửi đến AI.
- Xử lý các trường hợp:
  - Timeout.
  - Rate Limit.
  - Lỗi kết nối.
  - Response không hợp lệ.
  - Dịch vụ AI tạm thời không khả dụng.

---

# 🧪 Kiểm thử

Dự án bao gồm các tài liệu kiểm thử:

- `docs/Management_Test_Cases.md`
- `docs/AI_Test_Cases.md`
- `docs/AI_Code_Review.md`
- `docs/AI_Prompt_Testing.md`

Các nội dung kiểm thử gồm:

- Kiểm thử chức năng quản lý.
- Kiểm thử các chức năng AI.
- Kiểm thử lỗi.
- Kiểm thử bảo mật XSS.
- Review và cải thiện code bằng AI.

---

# 📂 Cấu trúc chính

```
CinemaBooking
│
├── Controllers
├── Models
├── Data
├── Services
│   └── AI
├── Prompts
├── Views
├── ViewModels
├── docs
└── wwwroot
```

---

# ⚙️ Cấu hình chạy dự án

## 1. Clone repository

```bash
git clone https://github.com/truongit8626-hue/CinemaBooking.git
```

## 2. Cấu hình Database

Cấu hình SQL Server trong:

```
appsettings.json
```

## 3. Cấu hình Gemini API Key

Sử dụng ASP.NET Core User Secrets:

```
Gemini:ApiKey
```

## 4. Chạy Migration Database

Thực hiện migration để tạo cơ sở dữ liệu.

## 5. Chạy Project

Mở project bằng Visual Studio và chạy ứng dụng.

---

 ## Tài khoản kiểm thử

Admin:
Email: hoanhue@gmail.com
Password: Truong86@

User:
Email: hoanhoan@gmail.com
Password: Truong86@


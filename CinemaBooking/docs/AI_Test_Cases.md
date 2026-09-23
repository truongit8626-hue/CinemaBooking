# AI Test Cases - CinemaBooking

## 1. Mục đích

Tài liệu này ghi nhận các test case cho các chức năng AI được tích hợp trong hệ thống CinemaBooking.

Các chức năng AI được kiểm thử:

1. AI đề xuất phim cá nhân hóa.
2. AI Chatbot hỗ trợ khách hàng.
3. AI dự báo doanh thu, số vé và tỷ lệ lấp đầy.
4. GeminiService và các trường hợp lỗi/giới hạn.

Phương pháp kiểm thử:

- Manual testing trên giao diện web.
- Kiểm thử trường hợp đúng.
- Kiểm thử trường hợp sai.
- Kiểm thử trường hợp biên.
- Kiểm tra xử lý lỗi và giới hạn AI.
- Kiểm tra quyền truy cập và bảo mật dữ liệu.

---

# 2. Môi trường kiểm thử

| Thành phần | Giá trị |
|---|---|
| Framework | ASP.NET Core MVC |
| Target Framework | .NET 10 |
| Database | SQL Server - CinemaDB |
| AI Provider | Google Gemini API |
| AI Model | gemini-3.5-flash-lite |
| Authentication | ASP.NET Core Identity |
| AI Recommendation | MovieController |
| AI Chatbot | AIChatController |
| AI Forecast | AIForecastController |
| AI Service | GeminiService |
| Test method | Manual testing |

---

# 3. Test case - AI đề xuất phim

## TC-AI-REC-01 - Đề xuất phim với người dùng có sở thích

**Mục tiêu:**  
Kiểm tra AI có sử dụng sở thích thể loại của người dùng để đề xuất phim.

**Điều kiện trước:**
- Người dùng đã đăng nhập.
- Người dùng đã chọn thể loại yêu thích khi đăng ký.
- Database có phim thuộc các thể loại tương ứng.

**Thao tác:**
1. Đăng nhập tài khoản người dùng.
2. Mở chức năng "Đề xuất phim".
3. Chờ AI xử lý.

**Kết quả mong đợi:**
- Trang đề xuất phim hiển thị bình thường.
- AI trả về danh sách phim phù hợp.
- Mỗi phim phải tồn tại trong CinemaBooking.
- Không xuất hiện MovieId không tồn tại trong database.
- Số lượng đề xuất không vượt quá 5 phim.

**Kết quả thực tế:**  
AI trả về danh sách phim dựa trên sở thích và dữ liệu hệ thống.

**Trạng thái:** PASS

---

## TC-AI-REC-02 - Đề xuất phim với người dùng chưa có lịch sử đặt vé

**Mục tiêu:**  
Kiểm tra AI vẫn hoạt động khi người dùng mới chưa có lịch sử đặt vé.

**Điều kiện trước:**
- Người dùng đã đăng nhập.
- Người dùng đã chọn thể loại yêu thích.
- Người dùng chưa có booking history.

**Thao tác:**
1. Đăng nhập tài khoản mới.
2. Mở chức năng đề xuất phim.
3. Chờ AI xử lý.

**Kết quả mong đợi:**
- Hệ thống không bị lỗi.
- AI sử dụng sở thích đã chọn.
- Không bắt buộc phải có lịch sử đặt vé.
- Chỉ đề xuất phim có trong database.

**Kết quả thực tế:**  
Hệ thống vẫn tạo được đề xuất dựa trên sở thích đã chọn.

**Trạng thái:** PASS

---

## TC-AI-REC-03 - Người dùng không có sở thích và lịch sử đặt vé

**Mục tiêu:**  
Kiểm tra trường hợp dữ liệu sở thích và lịch sử đặt vé đều rỗng.

**Điều kiện trước:**
- Người dùng đã đăng nhập.
- Không có UserPreference.
- Không có lịch sử đặt vé.

**Thao tác:**
1. Mở chức năng đề xuất phim.
2. Chờ AI xử lý.

**Kết quả mong đợi:**
- Hệ thống không crash.
- AI không tự tạo dữ liệu người dùng.
- Nếu không đủ cơ sở đề xuất, trả về danh sách rỗng hoặc thông báo phù hợp.

**Trạng thái:** PASS

---

## TC-AI-REC-04 - AI trả về MovieId không tồn tại

**Mục tiêu:**  
Kiểm tra hệ thống không hiển thị phim không tồn tại trong database.

**Dữ liệu kiểm thử:**  
MovieId = 999999

**Kết quả mong đợi:**
- MovieId không tồn tại bị loại bỏ.
- Không hiển thị phim giả.
- Ứng dụng không crash.

**Cơ chế bảo vệ:**  
Controller kiểm tra MovieId do AI trả về với MovieId thực tế trong database.

**Trạng thái:** PASS

---

## TC-AI-REC-05 - AI trả về quá 5 phim

**Mục tiêu:**  
Kiểm tra giới hạn số lượng recommendation.

**Kết quả mong đợi:**
- Hệ thống chỉ xử lý tối đa 5 phim.
- Không hiển thị quá số lượng cho phép.

**Cơ chế bảo vệ:**  
Controller sử dụng giới hạn `Take(5)`.

**Trạng thái:** PASS

---

## TC-AI-REC-06 - AI trả về JSON không hợp lệ

**Mục tiêu:**  
Kiểm tra hệ thống khi response Recommendation không đúng định dạng JSON.

**Kết quả mong đợi:**
- Trang không crash.
- Không hiển thị exception kỹ thuật.
- Hiển thị thông báo thân thiện.

**Thông báo mong đợi:**  
⚠️ AI trả về dữ liệu không đúng định dạng. Vui lòng thử lại sau.

**Trạng thái:** PASS - đã có cơ chế xử lý JsonException.

---

# 4. Test case - AI Chatbot

## TC-AI-CHAT-01 - Câu hỏi hợp lệ về phim

**Mục tiêu:**  
Kiểm tra chatbot trả lời câu hỏi về phim có trong database.

**Dữ liệu kiểm thử:**  
CinemaBooking hiện có những phim nào?

**Kết quả mong đợi:**
- Chatbot trả lời bằng tiếng Việt.
- Chỉ sử dụng dữ liệu phim từ database.
- Không tự tạo tên phim.

**Kết quả thực tế:**  
Chatbot trả về danh sách phim hiện có trong CinemaBooking.

**Trạng thái:** PASS

---

## TC-AI-CHAT-02 - Hỏi suất chiếu sắp tới

**Mục tiêu:**  
Kiểm tra chatbot sử dụng dữ liệu Showtime.

**Dữ liệu kiểm thử:**  
Avengers có suất chiếu nào sắp tới?

**Kết quả mong đợi:**
- Chatbot trả về các suất chiếu có trong database.
- Không tự tạo thời gian, phòng hoặc phim.

**Kết quả thực tế:**  
Chatbot trả về các suất chiếu Avengers có trong dữ liệu hệ thống.

**Trạng thái:** PASS

---

## TC-AI-CHAT-03 - Hỏi về phim không tồn tại

**Mục tiêu:**  
Kiểm tra chatbot không hallucination.

**Dữ liệu kiểm thử:**  
Phim XYZ123 có suất chiếu nào?

**Kết quả mong đợi:**
- Chatbot thông báo hệ thống chưa có thông tin phù hợp.
- Không tự tạo phim hoặc suất chiếu.

**Kết quả thực tế:**  
Chatbot không tự bịa thông tin.

**Trạng thái:** PASS

---

## TC-AI-CHAT-04 - Kiểm thử prompt chống hallucination

**Mục tiêu:**  
Kiểm tra chatbot khi hỏi phim không có trong hệ thống.

**Dữ liệu kiểm thử:**  
Hệ thống có phim Avatar 3 không? Nếu có, cho tôi biết suất chiếu gần nhất.

**Kết quả mong đợi:**
- Chỉ sử dụng dữ liệu backend cung cấp.
- Không sử dụng kiến thức bên ngoài.
- Không tự tạo suất chiếu.

**Kết quả thực tế:**  
Chatbot thông báo không có thông tin tương ứng trong hệ thống.

**Trạng thái:** PASS

---

## TC-AI-CHAT-05 - Câu hỏi rỗng

**Mục tiêu:**  
Kiểm tra validation đầu vào.

**Dữ liệu kiểm thử:**  
Chuỗi rỗng.

**Kết quả mong đợi:**
- Hệ thống yêu cầu nhập câu hỏi.
- Không gọi Gemini API.
- Không xảy ra lỗi.

**Trạng thái:** PASS

---

## TC-AI-CHAT-06 - Câu hỏi vượt giới hạn 2.000 ký tự

**Mục tiêu:**  
Kiểm tra input quá dài.

**Điều kiện:**  
Gửi câu hỏi dài hơn 2.000 ký tự.

**Kết quả mong đợi:**
- Hệ thống từ chối request quá dài.
- Không gọi Gemini không cần thiết.
- Hiển thị thông báo thân thiện.

**Kết quả thực tế:**  
Đã kiểm thử trực tiếp và hệ thống xử lý đúng.

**Trạng thái:** PASS

---

## TC-AI-CHAT-07 - Gemini timeout

**Mục tiêu:**  
Kiểm tra xử lý khi Gemini phản hồi quá lâu.

**Điều kiện:**  
Request vượt thời gian timeout.

**Kết quả mong đợi:**  
⚠️ AI phản hồi quá lâu. Vui lòng thử lại sau.

**Cơ chế:**  
GeminiService chuyển TaskCanceledException thành TimeoutException.

**Trạng thái:** PASS - đã có cơ chế xử lý.

---

## TC-AI-CHAT-08 - Gemini Rate Limit HTTP 429

**Mục tiêu:**  
Kiểm tra xử lý khi API Gemini giới hạn request.

**Điều kiện:**  
Gemini trả HTTP 429 Too Many Requests.

**Kết quả mong đợi:**  
⚠️ AI đang nhận quá nhiều yêu cầu. Vui lòng thử lại sau.

**Ghi chú:**  
Trong quá trình phát triển đã từng gặp lỗi 429 khi quota model bị vượt.

**Trạng thái:** PASS - đã có cơ chế xử lý.

---

## TC-AI-CHAT-09 - Gemini/API gặp lỗi khác

**Mục tiêu:**  
Kiểm tra khi dịch vụ AI không hoạt động bình thường.

**Kết quả mong đợi:**
- Không hiển thị stack trace.
- Không crash hệ thống.
- Hiển thị thông báo lỗi thân thiện.

**Trạng thái:** PASS - đã có generic error handling.

---

# 5. Test case - AI Forecast

## TC-AI-FOR-01 - Dự báo 7 ngày

**Mục tiêu:**  
Kiểm tra Forecast với khoảng thời gian 7 ngày.

**Dữ liệu kiểm thử:**  
7 ngày.

**Kết quả mong đợi:**
- AI phân tích dữ liệu bán vé thực tế.
- Hiển thị số vé dự kiến.
- Hiển thị doanh thu dự kiến.
- Hiển thị tỷ lệ lấp đầy dự kiến.
- Có đánh giá độ tin cậy.
- Có cảnh báo kết quả chỉ mang tính tham khảo.

**Kết quả thực tế:**  
AI tạo dự báo 7 ngày thành công.

**Trạng thái:** PASS

---

## TC-AI-FOR-02 - Dữ liệu Forecast ít

**Mục tiêu:**  
Kiểm tra AI khi dữ liệu lịch sử thưa thớt.

**Dữ liệu thực tế:**  
Trong 30 ngày chỉ có 2 ngày phát sinh giao dịch.

**Kết quả mong đợi:**
- AI phải cảnh báo dữ liệu ít.
- Đánh giá độ tin cậy thấp.
- Không khẳng định xu hướng chắc chắn.

**Kết quả thực tế:**  
AI đánh giá dữ liệu rất ít và độ tin cậy thấp.

**Trạng thái:** PASS

---

## TC-AI-FOR-03 - Dự báo 14 ngày

**Mục tiêu:**  
Kiểm tra lựa chọn thời gian 14 ngày.

**Kết quả mong đợi:**
- Hệ thống chấp nhận giá trị 14.
- AI tạo Forecast cho 14 ngày tiếp theo.

**Trạng thái:** PASS

---

## TC-AI-FOR-04 - Dự báo 30 ngày

**Mục tiêu:**  
Kiểm tra lựa chọn thời gian 30 ngày.

**Kết quả mong đợi:**
- Hệ thống chấp nhận giá trị 30.
- AI tạo Forecast cho 30 ngày tiếp theo.

**Trạng thái:** PASS

---

## TC-AI-FOR-05 - Giá trị days không hợp lệ

**Mục tiêu:**  
Kiểm tra giá trị ngoài danh sách 7, 14, 30.

**Dữ liệu kiểm thử:**  
days = 100

**Kết quả mong đợi:**
- Không sử dụng trực tiếp giá trị 100.
- Hệ thống đưa giá trị về mức hợp lệ mặc định.

**Trạng thái:** PASS - có validation phía controller.

---

## TC-AI-FOR-06 - Không có dữ liệu đặt vé

**Mục tiêu:**  
Kiểm tra trường hợp không có dữ liệu trong khoảng thời gian phân tích.

**Kết quả mong đợi:**
- Không gọi AI khi không có dữ liệu.
- Hiển thị thông báo cho Admin.

**Thông báo mong đợi:**  
⚠️ Chưa có đủ dữ liệu đặt vé trong 30 ngày gần nhất để thực hiện dự báo.

**Trạng thái:** PASS - đã có xử lý.

---

## TC-AI-FOR-07 - Gemini timeout trong Forecast

**Mục tiêu:**  
Kiểm tra xử lý timeout khi tạo dự báo.

**Kết quả mong đợi:**  
⚠️ AI phản hồi quá lâu. Vui lòng thử lại sau.

**Trạng thái:** PASS - đã có cơ chế xử lý.

---

## TC-AI-FOR-08 - Gemini Rate Limit trong Forecast

**Mục tiêu:**  
Kiểm tra xử lý HTTP 429.

**Kết quả mong đợi:**  
⚠️ AI đang nhận quá nhiều yêu cầu. Vui lòng thử lại sau.

**Trạng thái:** PASS - đã có cơ chế xử lý.

---

## TC-AI-FOR-09 - Không suy diễn quá mức khi dữ liệu ít

**Mục tiêu:**  
Kiểm tra prompt Forecast sau khi cải tiến.

**Điều kiện:**  
Dữ liệu lịch sử rất ít.

**Kết quả mong đợi:**
- Không sử dụng các kết luận quá mạnh khi dữ liệu không đủ.
- Có thể nói "chưa đủ dữ liệu để xác định xu hướng".
- Phân biệt dữ liệu thực tế, dự báo và nhận xét.

**Trạng thái:** PASS - prompt đã được cải thiện.

---

# 6. Test case - GeminiService

## TC-AI-SERVICE-01 - Không có API Key

**Mục tiêu:**  
Kiểm tra khi Gemini API Key không được cấu hình.

**Kết quả mong đợi:**
- Không gửi request.
- Throw lỗi cấu hình phù hợp.
- API Key không bị lộ.

**Thông báo nội bộ:**  
Không tìm thấy Gemini API Key.

**Trạng thái:** PASS - đã có validation.

---

## TC-AI-SERVICE-02 - System Prompt rỗng

**Mục tiêu:**  
Kiểm tra validation systemPrompt.

**Kết quả mong đợi:**
- Không gọi Gemini API.
- Request bị từ chối.

**Trạng thái:** PASS

---

## TC-AI-SERVICE-03 - User Prompt rỗng

**Mục tiêu:**  
Kiểm tra validation userPrompt.

**Kết quả mong đợi:**
- Không gọi Gemini API.
- Request bị từ chối.

**Trạng thái:** PASS

---

## TC-AI-SERVICE-04 - Prompt vượt giới hạn

**Mục tiêu:**  
Kiểm tra dữ liệu gửi tới AI quá lớn.

**Điều kiện:**  
System prompt hoặc user prompt vượt 100.000 ký tự.

**Kết quả mong đợi:**
- Không gửi dữ liệu quá lớn tới Gemini.
- Throw lỗi phù hợp.
- Controller chuyển thành thông báo thân thiện.

**Trạng thái:** PASS

---

## TC-AI-SERVICE-05 - Gemini trả response rỗng

**Mục tiêu:**  
Kiểm tra response body rỗng.

**Kết quả mong đợi:**
- Hệ thống phát hiện response rỗng.
- Không tiếp tục parse dữ liệu.

**Trạng thái:** PASS - đã có validation.

---

## TC-AI-SERVICE-06 - Response không có candidates

**Mục tiêu:**  
Kiểm tra JSON có cấu trúc không hợp lệ.

**Kết quả mong đợi:**
- Hệ thống phát hiện response không hợp lệ.
- Không crash.

**Trạng thái:** PASS

---

## TC-AI-SERVICE-07 - Response không có content hoặc parts

**Mục tiêu:**  
Kiểm tra response thiếu thành phần cần thiết.

**Kết quả mong đợi:**
- Hệ thống phát hiện dữ liệu không hợp lệ.
- Trả lỗi nội bộ phù hợp.

**Trạng thái:** PASS

---

## TC-AI-SERVICE-08 - Response không có text

**Mục tiêu:**  
Kiểm tra candidate không có nội dung văn bản.

**Kết quả mong đợi:**
- Không trả về chuỗi null.
- Hệ thống xử lý lỗi phù hợp.

**Trạng thái:** PASS

---

# 7. Test case - Bảo mật và quyền truy cập AI

## TC-AI-SEC-01 - User thường truy cập AI Forecast

**Mục tiêu:**  
Kiểm tra Forecast chỉ dành cho Admin.

**Điều kiện:**  
Đăng nhập bằng tài khoản User thường.

**Kết quả mong đợi:**
- User thường không được phép sử dụng Forecast.
- Authorization được áp dụng.

**Cơ chế:**  
Authorize Role Admin.

**Trạng thái:** PASS

---

## TC-AI-SEC-02 - Admin truy cập AI Forecast

**Mục tiêu:**  
Kiểm tra Admin có thể sử dụng Forecast.

**Kết quả mong đợi:**
- Admin mở được Forecast.
- Có thể chọn 7, 14 hoặc 30 ngày.

**Kết quả thực tế:**  
Admin sử dụng Forecast thành công.

**Trạng thái:** PASS

---

## TC-AI-SEC-03 - Recommendation chỉ sử dụng dữ liệu đúng người dùng

**Mục tiêu:**  
Kiểm tra dữ liệu cá nhân không bị trộn giữa các tài khoản.

**Kết quả mong đợi:**
- Booking history được lọc theo UserId hiện tại.
- UserPreference thuộc tài khoản hiện tại.
- Không sử dụng lịch sử của tài khoản khác.

**Trạng thái:** PASS - có kiểm soát UserId phía backend.

---

## TC-AI-SEC-04 - Chatbot không gửi dữ liệu cá nhân

**Mục tiêu:**  
Kiểm tra dữ liệu gửi cho Chatbot.

**Kết quả mong đợi:**
- Chỉ gửi Movies và Showtimes cần thiết.
- Không gửi password.
- Không gửi API Key.
- Không gửi thông tin tài khoản không liên quan.

**Trạng thái:** PASS

---

## TC-AI-SEC-05 - API Key không hard-code trong source

**Mục tiêu:**  
Kiểm tra bảo vệ Gemini API Key.

**Kết quả mong đợi:**
- API Key nằm trong User Secrets.
- Không xuất hiện API Key thật trong source code.
- Không commit secrets.json vào Git.

**Trạng thái:** PASS

---

# 8. Các test thực tế đã chạy trực tiếp

Các trường hợp sau đã được chạy trực tiếp trong quá trình phát triển:

1. Chatbot hỏi suất chiếu Avengers.
2. Chatbot hỏi phim không có trong hệ thống.
3. Chatbot input dài hơn 2.000 ký tự.
4. AI Recommendation với sở thích người dùng.
5. AI Recommendation kết hợp UserPreference và lịch sử booking.
6. AI Forecast 7 ngày.
7. Forecast với dữ liệu lịch sử chỉ có 2 ngày giao dịch.
8. Gemini từng trả HTTP 429 khi model cũ vượt quota.
9. Forecast hiển thị cảnh báo kết quả chỉ mang tính tham khảo.

**Kết quả:**  
Các luồng trên hoạt động theo yêu cầu sau khi hoàn thiện xử lý lỗi và prompt.

---

# 9. Các trường hợp được kiểm tra bằng code

Một số lỗi khó hoặc không nên cố tình tái tạo nhiều lần do giới hạn Gemini Free Tier được kiểm tra bằng code:

- Timeout.
- HTTP 429.
- HTTP error khác.
- Response body rỗng.
- Response JSON sai cấu trúc.
- Response thiếu candidates.
- Response thiếu text.
- Prompt vượt 100.000 ký tự.
- Không có API Key.

Các trường hợp này có validation và exception handling tương ứng trong GeminiService và các Controller.

---

# 10. Phân loại kiểm thử

## Trường hợp đúng

Đã kiểm thử:

- Recommendation với sở thích người dùng.
- Chatbot hỏi phim.
- Chatbot hỏi suất chiếu.
- Forecast 7 ngày.
- Forecast 14 ngày.
- Forecast 30 ngày.
- Admin sử dụng Forecast.

## Trường hợp sai

Đã kiểm thử hoặc có cơ chế xử lý:

- Phim không tồn tại.
- Câu hỏi rỗng.
- MovieId AI trả về không tồn tại.
- JSON Recommendation sai.
- Không có dữ liệu Forecast.
- API trả lỗi.
- Không có API Key.
- User thường truy cập Forecast.

## Trường hợp biên

Đã kiểm thử hoặc có validation:

- Câu hỏi vượt 2.000 ký tự.
- Prompt vượt 100.000 ký tự.
- Forecast với rất ít dữ liệu.
- Forecast với days không hợp lệ.
- Gemini timeout.
- Gemini HTTP 429.
- Response rỗng.
- Response sai cấu trúc.

---

# 11. Tổng hợp kết quả

| Nhóm | Số test case | Kết quả |
|---|---:|---|
| AI Recommendation | 6 | PASS |
| AI Chatbot | 9 | PASS |
| AI Forecast | 9 | PASS |
| GeminiService | 8 | PASS |
| Security / Authorization | 5 | PASS |
| Tổng cộng | 37 | PASS hoặc có cơ chế xử lý |

Lưu ý:  
Các test liên quan timeout, response lỗi và một số lỗi API được xác nhận thông qua code handling hoặc sự kiện đã gặp trong quá trình phát triển, không phải tất cả đều được cố tình tái tạo nhiều lần.

---

# 12. Kết luận

Ba chức năng AI chính của CinemaBooking đã được kiểm thử:

1. AI Recommendation.
2. AI Chatbot.
3. AI Forecast.

Quá trình kiểm thử bao gồm:

- Trường hợp đúng.
- Trường hợp sai.
- Trường hợp biên.
- Validation input.
- Kiểm soát dữ liệu hệ thống.
- Xử lý timeout.
- Xử lý rate limit.
- Xử lý response rỗng.
- Xử lý response sai cấu trúc.
- Kiểm soát quyền truy cập.
- Bảo vệ API Key.
- Kiểm tra AI không tự tạo dữ liệu ngoài hệ thống.

Các chức năng AI chính hoạt động bình thường trong các trường hợp đã kiểm thử.

Những tình huống lỗi khó tái tạo hoặc có nguy cơ tiêu tốn quota Gemini Free Tier đã được kiểm tra thông qua cơ chế xử lý trong code.

Tài liệu này cùng với `Management_Test_Cases.md` được sử dụng làm bằng chứng cho Criterion 8 - Kiểm thử chức năng quản lý và AI.
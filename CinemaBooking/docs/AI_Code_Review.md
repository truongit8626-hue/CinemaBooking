# AI Code Review - CinemaBooking

## 1. Mục đích

Tài liệu này ghi nhận các trường hợp AI được sử dụng để review code,
phát hiện lỗi, đề xuất cải thiện và hỗ trợ refactor trong hệ thống CinemaBooking.

Các nội dung review tập trung vào:

- Logic nghiệp vụ.
- Xử lý lỗi.
- Bảo mật.
- Validation.
- Quan hệ dữ liệu.
- Chất lượng code.
- Trải nghiệm người dùng.

---

# 2. Review 1 - Xóa Showtime bị lỗi khóa ngoại

## Vấn đề ban đầu

Khi Admin tạo một suất chiếu mới, hệ thống tự động tạo danh sách Seat
cho suất chiếu đó.

Tuy nhiên khi xóa Showtime chưa có vé đặt, hệ thống hiển thị:

> Không thể xóa! Suất chiếu đã có ghế hoặc dữ liệu liên quan.

## Phân tích bằng AI

AI review code và phát hiện:

- Showtime mới tạo luôn có Seat.
- Quan hệ Seat -> Showtime đang sử dụng DeleteBehavior.Restrict.
- Code DeleteShowtime xóa Showtime trực tiếp mà chưa xóa Seat.
- Vì vậy database từ chối xóa do khóa ngoại.

## Cải thiện

Logic xóa được thay đổi:

1. Kiểm tra Showtime có Booking hay chưa.
2. Nếu đã có Booking thì không cho xóa.
3. Nếu chưa có Booking thì lấy danh sách Seat của Showtime.
4. Xóa Seat trước.
5. Sau đó xóa Showtime.

## Kết quả

- Showtime chưa có Booking có thể xóa bình thường.
- Showtime đã có Booking vẫn được bảo vệ.
- Không còn lỗi khóa ngoại.

**Trạng thái:** Đã sửa và test PASS.

---

# 3. Review 2 - Lỗi Session trong Payment

## Vấn đề ban đầu

Sau khi người dùng chọn ghế và bấm "Tôi đã thanh toán",
hệ thống đôi khi hiển thị:

> Session hết hạn

## Phân tích bằng AI

AI review BookingController và Payment.cshtml và phát hiện:

- Book() lưu seatIds và showtimeId vào Session.
- ConfirmPayment() phụ thuộc hoàn toàn vào Session.
- Nếu Session không còn dữ liệu, ConfirmPayment() không thể tạo Booking.
- Các hidden input trong Payment ban đầu không cung cấp đúng dữ liệu cần thiết.

## Cải thiện

Luồng thanh toán được refactor:

- showtimeId được gửi trực tiếp từ Payment form.
- seatIds được gửi trực tiếp từ Payment form.
- ConfirmPayment nhận dữ liệu từ request.
- Session chỉ còn vai trò dữ liệu tạm.
- Sau khi thanh toán thành công, Session được xóa.

## Kết quả

Luồng mới:

Chọn ghế
→ Payment
→ ConfirmPayment
→ tạo Booking
→ tạo BookingDetail
→ hiển thị Ticket.

**Trạng thái:** Đã sửa và test PASS.

---

# 4. Review 3 - Xử lý lỗi Gemini API

## Vấn đề ban đầu

Khi Gemini API gặp lỗi, hệ thống có nguy cơ:

- Timeout.
- HTTP 429 Rate Limit.
- Response rỗng.
- Response sai cấu trúc.
- Prompt quá dài.
- Lỗi kết nối.
- Hiển thị lỗi kỹ thuật trực tiếp cho người dùng.

## Phân tích bằng AI

AI review GeminiService và các AI Controller.

Các điểm cần cải thiện:

- Cần có timeout rõ ràng.
- Cần xử lý riêng HTTP 429.
- Cần kiểm tra response trước khi đọc candidates.
- Cần validation prompt.
- Controller cần chuyển exception thành thông báo thân thiện.

## Cải thiện

GeminiService được bổ sung:

- Timeout 100 giây.
- Xử lý TaskCanceledException.
- Xử lý HttpRequestException.
- Xử lý HTTP 429.
- Kiểm tra response body rỗng.
- Kiểm tra candidates.
- Kiểm tra content.
- Kiểm tra parts.
- Kiểm tra text.
- Giới hạn prompt 100.000 ký tự.

Các Controller được bổ sung catch để hiển thị thông báo thân thiện.

## Kết quả

Hệ thống không crash khi Gemini gặp các lỗi chính.

**Trạng thái:** Đã cải thiện.

---

# 5. Review 4 - Prompt Forecast suy diễn quá mức

## Vấn đề ban đầu

Khi dữ liệu lịch sử chỉ có 2 ngày giao dịch,
AI Forecast từng sử dụng nhận xét:

> Nhu cầu đặt vé gần như đóng băng.

Nhận xét này quá mạnh so với lượng dữ liệu thực tế.

## Phân tích bằng AI

AI nhận thấy:

- Dữ liệu lịch sử quá ít.
- Không đủ cơ sở thống kê để kết luận xu hướng mạnh.
- Prompt chưa giới hạn các suy luận kinh doanh quá mức.

## Cải thiện

ForecastPrompt được bổ sung quy tắc:

- Không suy diễn nguyên nhân nếu dữ liệu không chứng minh.
- Không sử dụng các kết luận quá mạnh như "đóng băng", "khủng hoảng".
- Nếu dữ liệu ít phải nói "chưa đủ dữ liệu để xác định xu hướng".
- Phân biệt dữ liệu thực tế, dự báo và nhận xét.
- Kết quả chỉ mang tính tham khảo.

## Kết quả

Forecast thận trọng hơn khi dữ liệu lịch sử ít.

**Trạng thái:** Đã cải thiện prompt.

---

# 6. Review 5 - Kiểm tra dữ liệu Recommendation

## Vấn đề

AI có khả năng trả về MovieId không tồn tại trong database.

Nếu tin trực tiếp response của AI, hệ thống có thể hiển thị dữ liệu giả
hoặc gây lỗi.

## Phân tích bằng AI

AI đề xuất không sử dụng trực tiếp MovieId do model sinh ra.

## Cải thiện

Backend thực hiện:

1. Parse JSON từ Gemini.
2. Lấy MovieId.
3. Đối chiếu MovieId với database.
4. Chỉ giữ phim thực sự tồn tại.
5. Giới hạn tối đa 5 phim.

## Kết quả

AI không thể trực tiếp tạo một phim giả để hiển thị cho người dùng.

**Trạng thái:** Đã áp dụng.

---

# 7. Review 6 - Giới hạn input Chatbot

## Vấn đề

Nếu người dùng gửi nội dung quá dài:

- Tăng token sử dụng.
- Tốn quota Gemini.
- Tăng thời gian phản hồi.
- Có thể vượt giới hạn API.

## Phân tích bằng AI

AI đề xuất validation đầu vào trước khi gọi Gemini.

## Cải thiện

Chatbot giới hạn câu hỏi tối đa 2.000 ký tự.

Nếu vượt giới hạn:

- Không gọi Gemini.
- Hiển thị thông báo yêu cầu rút gọn câu hỏi.

## Kết quả

Giảm request không cần thiết và bảo vệ quota.

**Trạng thái:** Đã test PASS.

---

# 8. Review 7 - Nguy cơ XSS trong AI Chatbot

## Vấn đề được phát hiện

Trong giao diện Chatbot, JavaScript đang sử dụng innerHTML để chèn:

- Nội dung người dùng nhập.
- Nội dung AI trả về.

Ví dụ dạng:

messages.innerHTML += `<div>${message}</div>`;

và:

messages.innerHTML += `<div>${data.message}</div>`;

## Nguy cơ

Nếu message chứa HTML hoặc script độc hại,
trình duyệt có thể render trực tiếp nội dung đó.

Đây là nguy cơ Cross-Site Scripting (XSS).

## Đề xuất cải thiện

Không nên đưa trực tiếp dữ liệu người dùng hoặc AI vào innerHTML.

Nên:

- Tạo element bằng document.createElement().
- Dùng textContent để gán nội dung.
- Chỉ sử dụng innerHTML cho HTML cố định do hệ thống kiểm soát.

## Kết quả sau cải thiện

Giao diện Chatbot đã được refactor:

- Không còn chèn dữ liệu người dùng bằng `innerHTML`.
- Không còn chèn nội dung AI bằng `innerHTML`.
- Sử dụng `document.createElement()` để tạo phần tử.
- Sử dụng `textContent` để hiển thị dữ liệu động.

Đã kiểm thử bằng payload:

`<img src=x onerror=alert('XSS')>`

Kết quả:

- Payload chỉ hiển thị dưới dạng văn bản.
- Không thực thi JavaScript.
- Không xuất hiện popup XSS.

**Trạng thái:** ĐÃ SỬA VÀ TEST PASS.
---

# 9. Tổng hợp kết quả AI Code Review

| Nội dung review | Kết quả |
|---|---|
| Xóa Showtime và Seat | Đã sửa |
| Payment phụ thuộc Session | Đã sửa |
| Gemini timeout / 429 / response lỗi | Đã sửa |
| Forecast suy diễn quá mức | Đã sửa |
| Recommendation MovieId giả | Đã bảo vệ |
| Chatbot input quá dài | Đã giới hạn |
| XSS trong Chatbot | Đã sửa và test PASS |
---

# 10. Kết luận

AI đã được sử dụng trong quá trình phát triển CinemaBooking để review
code và hỗ trợ cải thiện chất lượng hệ thống.

AI không chỉ hỗ trợ tạo code mà còn được sử dụng để:

- Phân tích lỗi.
- Review logic nghiệp vụ.
- Phát hiện lỗi khóa ngoại.
- Cải thiện luồng thanh toán.
- Cải thiện xử lý lỗi API.
- Cải thiện prompt.
- Validation dữ liệu.
- Phát hiện nguy cơ bảo mật XSS.
- Đề xuất refactor.

Các đề xuất quan trọng đã được kiểm thử lại sau khi áp dụng.

Tài liệu này được sử dụng làm bằng chứng cho Criterion 9:
Review code và cải thiện chất lượng bằng AI.
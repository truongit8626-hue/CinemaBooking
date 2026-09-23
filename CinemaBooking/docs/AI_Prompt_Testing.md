# AI Prompt Testing & Optimization

## 1. Mục tiêu

Thử nghiệm và cải tiến prompt cho chức năng AI Chatbot của hệ thống CinemaBooking.

Mục tiêu:
- Kiểm tra khả năng sử dụng dữ liệu thực tế từ hệ thống.
- Giảm nguy cơ AI tự bịa thông tin.
- Kiểm tra cách AI xử lý khi dữ liệu không tồn tại.
- Cải thiện ràng buộc dữ liệu trong System Prompt và User Prompt.

---

## 2. Vòng thử nghiệm 1 — Prompt cơ bản

### Prompt

Sử dụng phiên bản prompt Chatbot ban đầu.

Prompt yêu cầu AI:
- Trả lời bằng tiếng Việt.
- Không tự bịa thông tin.
- Nếu không có đủ dữ liệu thì thông báo cho người dùng.
- Không tiết lộ dữ liệu cá nhân hoặc thông tin nội bộ.

### Test case

Câu hỏi:

> Hệ thống có phim Avatar 3 không? Nếu có, cho tôi biết suất chiếu gần nhất.

### Kết quả thực tế

AI trả lời:

> Hiện tại hệ thống CinemaBooking chưa có thông tin về phim Avatar 3 cũng như suất chiếu của phim này ạ.

### Đánh giá

- Không tự tạo thông tin về Avatar 3.
- Không tự tạo suất chiếu.
- AI biết thông báo khi hệ thống không có dữ liệu.
- Tuy nhiên, ràng buộc về nguồn dữ liệu chưa được mô tả thật chi tiết.

### Kết luận

Prompt cơ bản hoạt động tốt trong trường hợp dữ liệu không tồn tại, nhưng cần tăng ràng buộc để AI chỉ sử dụng dữ liệu do Backend cung cấp.

---

## 3. Vòng thử nghiệm 2 — Tăng ràng buộc dữ liệu

### Cải tiến prompt

Bổ sung các yêu cầu:

- Chỉ sử dụng thông tin trong dữ liệu CinemaBooking do Backend cung cấp.
- Không sử dụng kiến thức bên ngoài để bổ sung thông tin.
- Không tự suy đoán hoặc tạo tên phim, mã phim, phòng chiếu, ngày giờ hoặc suất chiếu.
- Nếu phim không xuất hiện trong dữ liệu thì phải nói rõ hệ thống chưa có thông tin.
- Nếu phim có trong dữ liệu nhưng không có suất chiếu phù hợp thì phải nói rõ.
- Không biến suy đoán thành thông tin thực tế.

User Prompt cũng được bổ sung các bước:
1. Đối chiếu câu hỏi với dữ liệu hệ thống.
2. Chỉ sử dụng thông tin xuất hiện trong dữ liệu.
3. Không sử dụng kiến thức bên ngoài.
4. Không tự tạo thông tin.
5. Thông báo rõ khi dữ liệu không đủ.

### Test case

Sử dụng lại cùng câu hỏi của Vòng 1 để so sánh:

> Hệ thống có phim Avatar 3 không? Nếu có, cho tôi biết suất chiếu gần nhất.

### Kết quả thực tế

AI trả lời:

> Hiện tại hệ thống CinemaBooking chưa có thông tin về phim Avatar 3 cũng như các suất chiếu cho phim này.

### Đánh giá

- Không tự tạo thông tin về Avatar 3.
- Không tự tạo suất chiếu.
- Trả lời rõ ràng hơn về việc hệ thống chưa có dữ liệu.
- Prompt có ràng buộc nguồn dữ liệu cụ thể hơn so với Vòng 1.

### Kết luận

Phiên bản prompt mới tăng khả năng kiểm soát dữ liệu và giảm nguy cơ AI sử dụng kiến thức bên ngoài để trả lời.

---

## 4. Vòng thử nghiệm 3 — Kiểm tra dữ liệu có thật trong hệ thống

### Mục tiêu

Sau khi kiểm tra trường hợp không có dữ liệu, tiến hành kiểm tra trường hợp phim có trong CinemaBooking và có suất chiếu thực tế.

### Prompt sử dụng

Giữ phiên bản prompt đã cải tiến ở Vòng 2.

### Test case

Câu hỏi:

> Avengers có suất chiếu nào sắp tới?

### Kết quả thực tế

AI trả về:

1. Phòng 2:
   - Bắt đầu: 00:50 ngày 25/09/2026
   - Kết thúc: 14:50 ngày 25/09/2026

2. Phòng 3:
   - Bắt đầu: 18:30 ngày 28/09/2026
   - Kết thúc: 20:30 ngày 28/09/2026

### Đánh giá

- AI trả về thông tin suất chiếu có trong dữ liệu hệ thống.
- Kết quả bao gồm phòng chiếu và thời gian.
- Prompt cải tiến vẫn cho phép AI khai thác dữ liệu thực tế thay vì chỉ từ chối câu hỏi.
- Có thể sử dụng cùng cơ chế kiểm soát dữ liệu cho các câu hỏi về phim và suất chiếu khác.

### Lưu ý

Suất chiếu Phòng 2 có khoảng thời gian bắt đầu và kết thúc dài bất thường. Đây là vấn đề cần kiểm tra ở dữ liệu nghiệp vụ của hệ thống, không phải vấn đề của prompt.

---

## 5. So sánh các vòng thử nghiệm

| Tiêu chí | Vòng 1 | Vòng 2 | Vòng 3 |
|---|---|---|---|
| Không bịa phim không tồn tại | Đạt | Đạt | - |
| Không bịa suất chiếu | Đạt | Đạt | Đạt |
| Ràng buộc chỉ dùng dữ liệu Backend | Cơ bản | Chặt hơn | Chặt hơn |
| Xử lý dữ liệu không tồn tại | Đạt | Đạt | - |
| Khai thác dữ liệu phim có thật | Chưa kiểm tra | Chưa kiểm tra | Đạt |
| Đối chiếu dữ liệu hệ thống | Cơ bản | Rõ ràng | Rõ ràng |

---

## 6. Cải tiến sau thử nghiệm

Sau 3 vòng thử nghiệm, prompt Chatbot được cải tiến theo hướng:

1. Xác định rõ vai trò của AI.
2. Xác định rõ nguồn dữ liệu được phép sử dụng.
3. Không cho phép AI tự tạo tên phim, mã phim, phòng chiếu hoặc thời gian.
4. Yêu cầu AI xử lý rõ trường hợp không có dữ liệu.
5. Yêu cầu AI đối chiếu câu hỏi với dữ liệu Backend trước khi trả lời.
6. Không sử dụng kiến thức bên ngoài để bổ sung thông tin cho hệ thống.
7. Giữ câu trả lời ngắn gọn, rõ ràng bằng tiếng Việt.

---

## 7. Kết luận

Qua 3 vòng thử nghiệm, prompt Chatbot được cải tiến từ các quy tắc chung sang cơ chế kiểm soát dữ liệu cụ thể hơn.

Kết quả kiểm thử cho thấy AI:
- Không tự tạo phim hoặc suất chiếu không có trong dữ liệu.
- Có thể thông báo khi hệ thống chưa có thông tin.
- Có thể khai thác dữ liệu phim và suất chiếu thực tế được Backend cung cấp.

Phiên bản prompt sau cải tiến được sử dụng trong:

`Prompts/ChatbotPrompt.cs`
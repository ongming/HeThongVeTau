using Guna.UI2.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace YourNamespace
{
    public static class ModernGridStyle
    {
        public static void Apply(Guna2DataGridView grid)
        {
            // Nền trắng, viền sáng
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Color.FromArgb(240, 240, 240);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.EnableHeadersVisualStyles = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            // Header
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersHeight = 38;

            // Hàng
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.RowTemplate.Height = 42;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 252);


            // Ẩn đường viền cột đầu tiên
            grid.RowHeadersVisible = false;

            // Định dạng scrollbar sáng hơn
            grid.ScrollBars = ScrollBars.Both;
            grid.ThemeStyle.BackColor = Color.White;
            grid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
            grid.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;

            // Hover mượt
            grid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(248, 249, 252);
        }

        /// <summary>
        /// Đổi màu chữ của cột "Trạng thái" theo giá trị.
        /// </summary>
        //public static void HighlightStatus(Guna2DataGridView grid, string columnName = "TrangThai")
        //{
        //    foreach (DataGridViewRow row in grid.Rows)
        //    {
        //        if (row.Cells[columnName].Value == null) continue;

        //        string status = row.Cells[columnName].Value.ToString().Trim().ToLower();

        //        if (status.Contains("đã hủy"))
        //            row.Cells[columnName].Style.ForeColor = Color.FromArgb(220, 53, 69); // đỏ
        //        else if (status.Contains("đã hoàn") || status.Contains("đã thanh toán"))
        //            row.Cells[columnName].Style.ForeColor = Color.FromArgb(25, 135, 84); // xanh lá
        //        else if (status.Contains("đã xuất"))
        //            row.Cells[columnName].Style.ForeColor = Color.FromArgb(13, 110, 253); // xanh dương
        //        else
        //            row.Cells[columnName].Style.ForeColor = Color.FromArgb(108, 117, 125); // xám
        //    }
        //}
        public static void ApplyWithMiniActions(Guna2DataGridView grid)
        {
            // 🔹 Áp dụng style gốc
            Apply(grid);

            // 🔹 Nếu đã có cột thao tác thì không thêm nữa
            if (grid.Columns.Contains("Actions")) return;

            // Cột thao tác (rỗng vì chứa icon)
            DataGridViewImageColumn viewCol = new DataGridViewImageColumn();
            viewCol.Name = "View";
            viewCol.HeaderText = "";
            viewCol.Image = CNPM.Properties.Resources.witness_1518564; // 👁 icon xem
            viewCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            viewCol.Width = 24;
            grid.Columns.Add(viewCol);

            DataGridViewImageColumn editCol = new DataGridViewImageColumn();
            editCol.Name = "Edit";
            editCol.HeaderText = "";
            editCol.Image = CNPM.Properties.Resources.edit; // ✏️ icon sửa
            editCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            editCol.Width = 24;
            grid.Columns.Add(editCol);

            DataGridViewImageColumn delCol = new DataGridViewImageColumn();
            delCol.Name = "Delete";
            delCol.HeaderText = "";
            delCol.Image = CNPM.Properties.Resources.delete; // 🗑 icon xóa
            delCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            delCol.Width = 24;
            grid.Columns.Add(delCol);

            // 🔸 Canh giữa 3 cột icon
            grid.Columns["View"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns["Edit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns["Delete"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 🔸 Giảm độ cao dòng để icon nhỏ gọn như web
            grid.RowTemplate.Height = 38;

            // 🔸 Header và nền
            grid.ColumnHeadersHeight = 36;
            grid.GridColor = Color.FromArgb(240, 240, 240);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 245, 245);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        public static void AddMiniActionColumns(Guna2DataGridView grid)
        {
            // 🧹 Xóa cột cũ nếu đã có
            if (grid.Columns.Contains("View")) return;

            // ❌ Không cho hiển thị dòng trống cuối
            grid.AllowUserToAddRows = false;

            // 👉 Cột xem (👁)
            var colView = new DataGridViewImageColumn
            {
                Name = "View",
                HeaderText = "",
                Image = CNPM.Properties.Resources.witness_1518564,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 22,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            // 👉 Cột sửa (✏️)
            var colEdit = new DataGridViewImageColumn
            {
                Name = "Edit",
                HeaderText = "",
                Image = CNPM.Properties.Resources.edit,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 22,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            // 👉 Cột xóa (🗑)
            var colDelete = new DataGridViewImageColumn
            {
                Name = "Delete",
                HeaderText = "",
                Image = CNPM.Properties.Resources.delete,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Width = 22,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            // 🔹 Thêm vào grid
            grid.Columns.Add(colView);
            grid.Columns.Add(colEdit);
            grid.Columns.Add(colDelete);

            // 🔹 Canh giữa icon
            grid.Columns["View"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns["Edit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns["Delete"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 🔹 Giữ kích thước icon cố định
            foreach (DataGridViewColumn col in grid.Columns)
            {
                if (col.Name == "View" || col.Name == "Edit" || col.Name == "Delete")
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                else
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // 🔹 Giảm chiều cao dòng cho đẹp
            grid.RowTemplate.Height = 36;

            // 🔹 Giãn icon nhẹ + hover hiệu ứng
            Image eye = CNPM.Properties.Resources.witness_1518564;
            Image pen = CNPM.Properties.Resources.edit;
            Image trash = CNPM.Properties.Resources.delete;

            Image eyeHover = CNPM.Properties.Resources.witness_1518564;     // 👁 xanh
            Image penHover = CNPM.Properties.Resources.edit;  // ✏️ cam
            Image trashHover = CNPM.Properties.Resources.delete; // 🗑 đỏ

            int hoveredCol = -1, hoveredRow = -1;

            grid.CellPainting += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == grid.Columns["View"].Index ||
                        e.ColumnIndex == grid.Columns["Edit"].Index ||
                        e.ColumnIndex == grid.Columns["Delete"].Index)
                    {
                        e.PaintBackground(e.CellBounds, true);

                        Image img = null;
                        var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewImageCell;
                        if (cell != null)
                        {
                            if (cell.OwningColumn.Name == "View")
                                img = (e.ColumnIndex == hoveredCol && e.RowIndex == hoveredRow) ? eyeHover : eye;
                            else if (cell.OwningColumn.Name == "Edit")
                                img = (e.ColumnIndex == hoveredCol && e.RowIndex == hoveredRow) ? penHover : pen;
                            else if (cell.OwningColumn.Name == "Delete")
                                img = (e.ColumnIndex == hoveredCol && e.RowIndex == hoveredRow) ? trashHover : trash;
                        }

                        if (img != null)
                        {
                            int size = 16;
                            int x = e.CellBounds.Left + (e.CellBounds.Width - size) / 2;
                            int y = e.CellBounds.Top + (e.CellBounds.Height - size) / 2;
                            e.Graphics.DrawImage(img, new Rectangle(x, y, size, size));
                        }

                        e.Handled = true;
                    }
                }
            };

            grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 &&
                    (e.ColumnIndex == grid.Columns["View"].Index ||
                     e.ColumnIndex == grid.Columns["Edit"].Index ||
                     e.ColumnIndex == grid.Columns["Delete"].Index))
                {
                    hoveredCol = e.ColumnIndex;
                    hoveredRow = e.RowIndex;
                    grid.InvalidateCell(e.ColumnIndex, e.RowIndex);
                }
            };

            grid.CellMouseLeave += (s, e) =>
            {
                if (hoveredCol != -1 && hoveredRow != -1)
                {
                    int c = hoveredCol;
                    int r = hoveredRow;
                    hoveredCol = hoveredRow = -1;
                    grid.InvalidateCell(c, r);
                }
            };
        }

        public static void ApplyPlaceholder(Guna2DateTimePicker picker, string placeholderText)
        {
            // === THIẾT LẬP GIAO DIỆN CƠ BẢN ===
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = " ";
            picker.FillColor = Color.White;
            picker.BorderRadius = 6;
            picker.ForeColor = Color.Black;
            picker.Checked = false;
            picker.Tag = null;

            // === NẾU KHÔNG CÓ PLACEHOLDER => CHỈ SET STYLE, KHÔNG VẼ GÌ CẢ ===
            if (string.IsNullOrWhiteSpace(placeholderText))
                return;

            // === SỰ KIỆN CHỌN GIÁ TRỊ: HIỂN THỊ NGÀY ===
            picker.ValueChanged += (s, e) =>
            {
                picker.Format = DateTimePickerFormat.Custom;
                picker.CustomFormat = "dd/MM/yyyy";
                picker.Tag = "selected";
                picker.Refresh();
            };

            // === SỰ KIỆN VẼ PLACEHOLDER ===
            picker.Paint += (s, e) =>
            {
                if (picker.Tag == null || (string)picker.Tag != "selected")
                {
                    string text = placeholderText.Trim();
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        SizeF textSize = e.Graphics.MeasureString(text, picker.Font);

                        float iconOffset = 28f; // lệch phải khỏi icon lịch
                        float x = iconOffset;
                        float y = (picker.Height - textSize.Height) / 2f + 1;

                        e.Graphics.DrawString(text, picker.Font, brush, x, y);
                    }
                }
            };
        }

        public static void ApplyModern(Guna2DataGridView grid)
        {
            // ======= GIAO DIỆN CHUNG =======
            grid.BackgroundColor = Color.White;
            grid.GridColor = Color.FromArgb(240, 242, 245);
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;

            // ======= HEADER =======
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersHeight = 40;

            // ======= DÒNG =======
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(30, 30, 30);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 245, 249);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 253, 253);
            grid.RowTemplate.Height = 42;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToResizeRows = false;

            // ======= BORDER & TONE =======
            grid.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;

            // ======= HIỆU ỨNG HOVER NHẸ =======
            grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
            };
            grid.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            };

            // ======= HIỂN THỊ SAO Ở CỘT 'DanhGia' =======
            grid.CellFormatting += (s, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name.Equals("DanhGia", StringComparison.OrdinalIgnoreCase) && e.Value != null)
                {
                    try
                    {
                        int soSao = Convert.ToInt32(e.Value);
                        if (soSao < 0) soSao = 0;
                        if (soSao > 5) soSao = 5;

                        // hiển thị ký tự sao vàng
                        e.Value = new string('★', soSao) + new string('☆', 5 - soSao);
                        e.CellStyle.ForeColor = Color.FromArgb(255, 193, 7);
                        e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        e.CellStyle.Font = new Font("Segoe UI Emoji", 11, FontStyle.Bold);
                        e.FormattingApplied = true;
                    }
                    catch { }
                }
            };

            // ======= TRẠNG THÁI (nếu có cột 'TrangThai') =======
            grid.CellFormatting += (s, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name.Equals("TrangThai", StringComparison.OrdinalIgnoreCase) && e.Value != null)
                {
                    string status = e.Value.ToString().Trim().ToLower();

                    if (status.Contains("đã"))
                    {
                        e.CellStyle.ForeColor = Color.White;
                        e.CellStyle.BackColor = Color.FromArgb(25, 135, 84); // xanh lá
                    }
                    else if (status.Contains("đang"))
                    {
                        e.CellStyle.ForeColor = Color.Black;
                        e.CellStyle.BackColor = Color.FromArgb(255, 220, 100); // vàng
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Color.Black;
                        e.CellStyle.BackColor = Color.FromArgb(230, 230, 230); // xám
                    }

                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                    e.FormattingApplied = true;
                }
            };
        }
        public static void ApplyActivityFeed(Guna2DataGridView grid)
        {
            // ====== NỀN & CƠ BẢN ======
            grid.BackgroundColor = Color.White; // nền trắng tinh
            grid.GridColor = Color.White; // bỏ luôn viền lưới
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.EnableHeadersVisualStyles = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToResizeColumns = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            // ====== NỀN DÒNG ======
            grid.DefaultCellStyle.BackColor = Color.White;               // nền trắng
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White; // tất cả trắng
            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // ✅ Bỏ hiệu ứng chọn (selection)
            grid.DefaultCellStyle.SelectionBackColor = Color.White;
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            // ====== CỘT & KÍCH THƯỚC ======
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowTemplate.Height = 40;
            grid.ScrollBars = ScrollBars.None;

            // ====== ẨN CỘT ĐẦU TIÊN (ID / STT) ======
            if (grid.Columns.Count > 0)
                grid.Columns[0].Visible = false;

            // ====== HIỆU ỨNG HOVER MÀU XANH NHẠT NHƯ FEED ======
            grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255); // xanh nhạt hover
            };
            grid.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            };

            // ====== FORMAT CỘT "ThoiGian" ======
            grid.CellFormatting += (s, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name.Equals("ThoiGian", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            };

            // ====== FORMAT CỘT "NguoiDung" ======
            grid.CellFormatting += (s, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name.Equals("NguoiDung", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
                    e.CellStyle.ForeColor = Color.Black;
                }
            };

            // ====== ĐỒNG BỘ THEME ======
            grid.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.None;
            grid.ThemeStyle.RowsStyle.BackColor = Color.White;
            grid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            grid.ThemeStyle.RowsStyle.SelectionBackColor = Color.White;
            grid.ThemeStyle.RowsStyle.SelectionForeColor = Color.Black;
        }


    }
}

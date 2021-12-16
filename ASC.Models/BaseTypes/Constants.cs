using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ASC.Models.BaseTypes
{
    public static class Constants
    {
        public const string Equal = "eq";
        public const string NotEqual = "ne";
        public const string GreaterThan = "gt";
        public const string GreaterThanOrEqual = "ge";
        public const string LessThan = "lt";
        public const string LessThanOrEqual = "le";
    }
    public enum Roles
    {
        Admin, Engineer, User
    }
    public enum MasterKeys
    {
        Giày, Màu
    }
    public enum Status
    {
        [Display(Name="Mới")] New,
        [Display(Name = "Hủy")] Denied,
        [Display(Name = "Chưa giải quyết")] Pending,
        [Display(Name = "Bắt đầu")] Initiated,
        [Display(Name = "Đang vận chuyển")] InProgress,
        [Display(Name = "Chưa được xác nhận")] PendingCustomerApproval,
        [Display(Name = "Thiếu thông tin")] RequestForInformation,
        [Display(Name = "Hoàn thành")] Completed
    }
}

﻿namespace ShoppingStore.Model.Dtos
{
    public class VnpayDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
    }
}
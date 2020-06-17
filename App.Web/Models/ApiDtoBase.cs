using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Web.Models {
    public class MessageRequestDto {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public string Date { get; set; }
    }

    public class PostMessageRequestDto<T> where T : new() {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public T Data { get; set; }
    }

    public class MessageResponseDto<T> : MessageResponseDto where T : new() {
        
        public T Data { get; set; }

        public MessageResponseDto() : base() { }
        public MessageResponseDto(MessageRequestDto req) : base(req) {
            
        }
        
    }

    public class MessageResponseDto { 
        public string ResponseMessage { get; set; }
        public int? ItemCount { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public int? PageCount { get; set; }
        public StatusCode StatusCode { get; set; }


        public MessageResponseDto() { }
        public MessageResponseDto(MessageRequestDto req) {
            this.PageSize = req.PageSize;
            this.PageNumber = req.PageNumber;
        }

        public void SetError(Exception e) {
            ResponseMessage = e.GetBaseException().Message;
            StatusCode = StatusCode.error;
        }
    }

    public enum StatusCode {
        success,
        failed,
        error
    }
}
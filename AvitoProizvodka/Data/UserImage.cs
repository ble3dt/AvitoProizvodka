//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AvitoProizvodka.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserImage
    {
        public int UserId { get; set; }
        public int ImageId { get; set; }
        public System.DateTime Timestamp { get; set; }
    
        public virtual Image Image { get; set; }
        public virtual User User { get; set; }
    }
}

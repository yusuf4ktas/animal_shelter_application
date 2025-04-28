using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace animal_shelter_app.Models
{

    [Table("article_informations")]

    public class ArticleInformations
    {
        [Key]
        [Column("article_id")] 
        public int ArticleId { get; set; }

        [Column("blog_title")]
        public string? BlogTitle { get; set; }

        [Column("blog_content")]
        public string? BlogContent { get; set; }

        [Column("blog_date")]
        public string? BlogDate { get; set; }

        [Column("blog_url")]
        public string? BlogUrl { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace animal_shelter_app.Models
{

    [Table("article_informations")]

    public class ArticleInformations
    {
        [Key]
        [Column("article_id")] //Have been added to match the id of any added texts. Restore the DLL accordingly.
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

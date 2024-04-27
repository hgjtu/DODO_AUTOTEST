using System.Threading.Tasks;
using Microsoft.Playwright;

namespace BigEcommerceApp.Tests.Models;

public class MainPage : BasePage
{
  public MainPage(IPage _page) : base(_page) {
    this.page = _page;
  }
}
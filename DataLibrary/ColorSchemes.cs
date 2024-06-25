using DataLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class ColorSchemes
    {
        private static ColorSchemes _Schemes;
        public static ColorSchemes Instance
        {
            get
            {
                if (_Schemes == null)
                {
                    _Schemes = new ColorSchemes();
                }
                return _Schemes;
            }
        }

        //BLACK & WHITE
        public string BaseLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #000000;
                                        --hover-bg: #2d2d2d;
                                        --hover-txt: #ffffff;
                                        --active-bg: #558395;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #ffffff74;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string BaseDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #ffffff;
                                        --hover-bg: #ffffff;
                                        --hover-txt: #1b1b1b;
                                        --active-bg: #558395;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #ffffff7f;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }


        //RED
        public string RedLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #C70005;
                                        --hover-bg: #C70005;
                                        --hover-txt: #ffffff;
                                        --active-bg: #FE474C;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #c7000556;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string RedDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #FD1D23;
                                        --hover-bg: #FD1D23;
                                        --hover-txt: #ffffff;
                                        --active-bg: #FE474C;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #c700058c;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }


        //ORANGE
        public string OrangeLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #c95700;
                                        --hover-bg: #c95700;
                                        --hover-txt: #ffffff;
                                        --active-bg: #FF9748;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #c9570056;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string OrangeDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #FF6F00;
                                        --hover-bg: #FF6F00;
                                        --hover-txt: #ffffff;
                                        --active-bg: #FF9748;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #c9570089;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }


        //YELLOW
        public string YellowLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #2b2b2b;
                                        --nav-text-color: #FFD436;
                                        --hover-bg: #FFD436;
                                        --hover-txt: #2b2b2b;
                                        --active-bg: #FFDC5C;
                                        --text-stroke: 1px 1px 0px #2b2b2b;
                                        --img-shadow: 0px 0px 11px 4px #ffd43660;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string YellowDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #1b1b1b;
                                        --nav-text-color: #FFD436;
                                        --hover-bg: #FFD436;
                                        --hover-txt: #1b1b1b;
                                        --active-bg: #FFDC5C;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #ffd43672;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }


        //GREEN
        public string GreenLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #0D8A00;
                                        --hover-bg: #0D8A00;
                                        --hover-txt: #ffffff;
                                        --active-bg: #69E25D;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #11b0005b;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string GreenDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #11B000;
                                        --hover-bg: #11B000;
                                        --hover-txt: #ffffff;
                                        --active-bg: #69E25D;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #11b0006b;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }


        //BLUE
        public string BlueLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #082D70;
                                        --hover-bg: #082D70;
                                        --hover-txt: #ffffff;
                                        --active-bg: #3B68B9;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #194fb168;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #87b2ff;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string BlueDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #4c82e6;
                                        --hover-bg: #194FB1;
                                        --hover-txt: #ffffff;
                                        --active-bg: #3B68B9;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #194fb183;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #87b2ff;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }


        //CYAN
        public string CyanLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #015B67;
                                        --hover-bg: #015B67;
                                        --hover-txt: #ffffff;
                                        --active-bg: #51B3C0;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #0b92a573;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string CyanDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #0B92A5;
                                        --hover-bg: #0B92A5;
                                        --hover-txt: #ffffff;
                                        --active-bg: #51B3C0;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #0b92a573;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }


        //PURPLE
        public string PurpleLight()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #ffffff;
                                        --gradient-dark: #d2d2d2;
                                        --dark-gray: #c1c1c1;
                                        --text-color: #000000;
                                        --header-bg: #9b9b9b;
                                        --list-border: #e9e9e9;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #7B0BDB;
                                        --hover-bg: #7B0BDB;
                                        --hover-txt: #ffffff;
                                        --active-bg: #b779ed;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #8c29e261;
                                        --form-bg: #dddddd;
                                        --table-stripe-light: #ffffff;
                                        --table-stripe-dark: #f1f1f1;
                                        --theme-icon-shade: brightness(75%);
                                        --color-scheme: light;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #c4c4c4a9;
                                        --inset-light: #e6e6e6;
                                    }</style>");
            return colorTheme;
        }
        public string PurpleDark()
        {
            var colorTheme = (@"<style type='text/css' id='colorTheme'>:root {
                                        --gradient-base: #474747;
                                        --gradient-dark: #2d2d2d;
                                        --dark-gray: #1b1b1b;
                                        --text-color: #ffffff;
                                        --header-bg: #111111;
                                        --list-border: #474747;
                                        --active-txt: #ffffff;
                                        --nav-text-color: #8C29E2;
                                        --hover-bg: #8C29E2;
                                        --hover-txt: #ffffff;
                                        --active-bg: #b779ed;
                                        --text-stroke: none;
                                        --img-shadow: 0px 0px 11px 4px #8c29e289;
                                        --form-bg: #696969;
                                        --table-stripe-light: #696969;
                                        --table-stripe-dark: #595959;
                                        --theme-icon-shade: brightness(100%);
                                        --color-scheme: dark;
                                        --logo-tint: #3B68B9;
                                        --logo-text-tint: #194FB1;
                                        --inset-dark: #0000007b;
                                        --inset-light: #797979ff;
                                    }</style>");
            return colorTheme;
        }
    }
}

# HitScoreVisualizer

### Changelog
2.0.2: Bug fixes and performance improvements. If you've been noticing lag, try disabling the fade option on all judgments in the config.
***
Colors the score popups that appear when hitting notes. Also adds judgment text above (or below) the score.

After installing the plugin, run the game once to generate the config file at `UserData/HitScoreVisualizerConfig.json` in your Beat Saber install folder. A graphical editor for this file is in the works; until then, editing the config is considered an advanced feature, as JSON is not particularly human-friendly. If you want to edit it anyway, here's the documentation:
* Remove the `isDefaultConfig` line. If this option is set to true, your config will get overwritten by the updated default config with every plugin update.
* Valid options for `displayMode` are "numeric" (displays only the score), "textOnly" (displays only the judgment text), "scoreOnTop" (displays the score above the judgment text), or any other string (displays the judgment text above the score).
* Put your judgments in descending order; the first one encountered with `threshold` <= the score earned for a note will be applied.
* Include exactly 4 numbers in each judgment's `color` array. These represent red, green, blue, and a 4th channel which, knowing Beat Saber's shaders, could be glow, transparency, or even both at once. Modify the 4th number if you like, but I can't say what effect it will have.
* If you include more or fewer than 4 numbers in a judgment's color array, your entire config will be ignored (but not overwritten).
* If you include the line `"fade": true` in a judgment, the color for scores earning that judgment will be interpolated between that judgment's color and the color of the previous judgment in the list, based on how close to the threshold for that judgment the score was. Use this to create a smooth gradient of color if you want one.
* If you enable `fade` for the first judgment in the list, the plugin and Beat Saber itself will quite possibly die in a fire if that judgment is ever earned. Don't enable `fade` for the first judgment in the list. You have been warned.
* Judgment text supports [TextMeshPro formatting!](http://digitalnativestudios.com/textmeshpro/docs/rich-text/)

If you make your own config, feel free to share it in #other-files!

Plugin originally requested by @AntRazor on the modding discord. Thanks go to @AntRazor and @wulkanat for input on the default config released with this update, as well as everyone in #mod-development and the entire server for the love and support. :')

We're on [GitHub!](https://github.com/artemiswkearney/HitScoreVisualizer/releases)

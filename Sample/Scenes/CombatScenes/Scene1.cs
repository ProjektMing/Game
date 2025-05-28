using Nez.BitmapFonts;
using Stg.Nez.Scenes.CombatScene;

namespace Sample.Scenes.CombatScenes;

public class Scene1(string name) : CombatBaseScene(name, BitmapFontLoader.LoadFontFromFile(Nez.Content.Fonts.Name));
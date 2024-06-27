import { ModRegistrar } from "cs2/modding";
import { MainContainerComponent } from "mods/MainContainer/MainContainer";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import { ZonePanelButton } from "mods/ZonePanelButton/ZonePanelButton";
import mod from "../mod.json";

const register: ModRegistrar = (moduleRegistry) => {
  console.log(mod.id + " UI module registering...");
  VanillaComponentResolver.setRegistry(moduleRegistry);

  moduleRegistry.append("Game", MainContainerComponent);

  moduleRegistry.extend(
    "game-ui/game/components/asset-menu/asset-category-tab-bar/asset-category-tab-bar.tsx",
    "AssetCategoryTabBar",
    ZonePanelButton
  );

  // This is just to verify using UI console that all the component registriations was completed.
  console.log(mod.id + " UI module registrations completed.");
};

export default register;

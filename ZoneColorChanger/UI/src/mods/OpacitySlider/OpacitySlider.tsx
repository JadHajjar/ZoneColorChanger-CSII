import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, UISound, FOCUS_AUTO } from "cs2/ui";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";

// This functions trigger an event on C# side and C# designates the method to implement.
const ColorMode$ = bindValue<number>(mod.id, "ColorMode", 0);

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

console.log(UISound);

export const OpacitySlider = (props: { value: number; trigger: string }) => {
  return (
    <div style={{ padding: "10rem" }}>
      <VanillaComponentResolver.instance.Slider
        value={props.value}
        start={0}
        end={1}
        onChange={(v) => trigger(mod.id, props.trigger, v)}
      ></VanillaComponentResolver.instance.Slider>
    </div>
  );
};

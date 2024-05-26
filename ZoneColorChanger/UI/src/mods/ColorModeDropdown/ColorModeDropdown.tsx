import { useValue, bindValue, trigger } from "cs2/api";
import { Theme } from "cs2/bindings";
import { getModule } from "cs2/modding";
import mod from "../../../mod.json";
import { Dropdown, DropdownItem, DropdownToggle, UISound, FOCUS_AUTO } from "cs2/ui";

const colorModes = ["Default", "Custom", "Deuteranopia", "Protanopia", "Tritanopia"];

// This functions trigger an event on C# side and C# designates the method to implement.
const ColorMode$ = bindValue<number>(mod.id, "ColorMode", 0);

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");

console.log(UISound);

export const ColorModeDropdown = () => {
  const ColorMode = useValue(ColorMode$);
  const dropDownItems = colorModes.map((mode, index) => (
    <DropdownItem<Number>
      theme={DropdownStyle}
      focusKey={FOCUS_AUTO}
      value={index}
      closeOnSelect={true}
      onToggleSelected={() => trigger(mod.id, "SetColorMode", index)}
      selected={true}
      sounds={{ select: "select-item" }}
    >
      {mode}
    </DropdownItem>
  ));

  return (
    <div style={{ padding: "5rem" }}>
      <Dropdown focusKey={FOCUS_AUTO} theme={DropdownStyle} content={dropDownItems}>
        <DropdownToggle>{colorModes[ColorMode]}</DropdownToggle>
      </Dropdown>
    </div>
  );
};

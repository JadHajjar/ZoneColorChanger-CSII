import { useValue, bindValue, trigger } from "cs2/api";
import { game } from "cs2/bindings";
import mod from "../../../mod.json";
import { useState, useRef } from "react";
import styles from "./mainContainer.module.scss";
import { Button, FOCUS_DISABLED, Tooltip } from "cs2/ui";
import { useLocalization } from "cs2/l10n";
import { ZonesContainer } from "mods/ZonesContainer/ZonesContainer";
import { ColorModeDropdown } from "mods/ColorModeDropdown/ColorModeDropdown";
import { OpacitySettings } from "mods/OpacitySettings/OpacitySettings";

// This functions trigger an event on C# side and C# designates the method to implement.
const MainPanelVisible$ = bindValue<boolean>(mod.id, "MainPanelVisible", false);

export const MainContainerComponent = () => {
  const { translate } = useLocalization();
  const isPhotoMode = useValue(game.activeGamePanel$)?.__Type == game.GamePanelType.PhotoMode;
  const MainPanelVisible = useValue(MainPanelVisible$);
  const containerRef = useRef(null);

  const [isMouseDown, SetMouseDown] = useState(true);

  function containerMouseDown(e: React.MouseEvent<HTMLDivElement>) {
    SetMouseDown(true);
  }

  function containerMouseMove(e: React.MouseEvent<HTMLDivElement>) {
    if (!isMouseDown) return;
    console.log(e.clientX + " " + e.clientY);
    console.log((containerRef.current as any).getBoundingClientRect());
  }

  function containerMouseUp(e: React.MouseEvent<HTMLDivElement>) {
    SetMouseDown(false);
  }

  if (isPhotoMode || !MainPanelVisible) return <></>;

  return (
    <div className={styles.container}>
      <div ref={containerRef} onMouseDown={containerMouseDown} onMouseMove={containerMouseDown} onMouseUp={containerMouseUp}>
        <div className={styles.dropdown}>
          <span>Mode:</span>
          <ColorModeDropdown />
        </div>

        <OpacitySettings />

        <ZonesContainer />

        <Tooltip tooltip={translate(`Tooltip.LABEL[${mod.id}.ClosePanel]`, "Close Panel")}>
          <Button
            className={styles.closeIcon}
            variant="icon"
            onSelect={() => trigger(mod.id, "SetMainPanelVisible", false)}
            focusKey={FOCUS_DISABLED}
          >
            <img style={{ maskImage: "url(Media/Glyphs/Close.svg)" }}></img>
          </Button>
        </Tooltip>
      </div>
    </div>
  );
};

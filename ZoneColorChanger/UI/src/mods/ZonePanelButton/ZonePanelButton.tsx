import { bindValue, trigger, useValue } from "cs2/api";
import { ModuleRegistryExtend } from "cs2/modding";
import styles from "./ZonePanelButton.module.scss";
import { Button } from "cs2/ui";
import mod from "mod.json";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";
import paintBrush from "image/paintBrush.svg";

const ZoneToolActive$ = bindValue(mod.id, "ZoneToolActive", false);
const MainPanelVisible$ = bindValue<boolean>(mod.id, "MainPanelVisible", false);

export const ZonePanelButton: ModuleRegistryExtend = (Component) => {
  return (props) => {
    const MainPanelVisible = useValue(MainPanelVisible$);
    const ZoneToolActive = useValue(ZoneToolActive$);
    const { children, ...otherProps } = props || {};

    if (!ZoneToolActive) return <Component {...otherProps}>{children}</Component>;

    return (
      <>
        <div className={styles.container}>
          <Button
            className={styles.button}
            variant="flat"
            onSelect={() => trigger(mod.id, "SetMainPanelVisible", !MainPanelVisible)}
            focusKey={VanillaComponentResolver.instance.FOCUS_DISABLED}
          >
            <img style={{ maskImage: `url('${paintBrush}')` }} />
            Edit Zone Colors
          </Button>
        </div>
        <Component {...otherProps}>{children}</Component>
      </>
    );
  };
};

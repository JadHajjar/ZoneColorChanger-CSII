import { bindValue, trigger, useValue } from "cs2/api";
import { Color, Theme } from "cs2/bindings";
import mod from "../../../mod.json";
import { getModule } from "cs2/modding";
import styles from "./ZoneItem.module.scss";
import { VanillaComponentResolver } from "../VanillaComponentResolver/VanillaComponentResolver";
import { Tooltip } from "cs2/ui";
import { ZoneInfoItem } from "domain/ZoneInfoItem";
import { useState } from "react";

export const ZoneItem = (props: { zone: ZoneInfoItem }) => {
  const [color, updateColor] = useState<Color>(props.zone.Color);

  return (
    <div className={styles.container}>
      <span className={styles.label}>{props.zone.PrefabName}</span>
      <VanillaComponentResolver.instance.ColorField
        value={props.zone.Color}
        className={styles.colorPicker}
        onChange={(e) => {
          trigger(mod.id, "SetZoneColor", props.zone.PrefabId, e);
          updateColor(e);
        }}
      ></VanillaComponentResolver.instance.ColorField>
    </div>
  );
};

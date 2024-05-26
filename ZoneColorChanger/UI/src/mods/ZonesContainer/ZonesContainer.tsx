import { bindValue, trigger, useValue } from "cs2/api";
import mod from "../../../mod.json";
import styles from "./ZonesContainer.module.scss";
import { ZoneItem } from "../ZoneItem/ZoneItem";
import { ZoneGroupItem } from "domain/ZoneGroupItem";
import { Section } from "mods/Section/Section";
import { Scrollable } from "cs2/ui";

const ZoneInfoList$ = bindValue<ZoneGroupItem[]>(mod.id, "ZoneInfoList");

export const ZonesContainer = () => {
  const ZoneInfoList = useValue(ZoneInfoList$);

  return (
    <div className={styles.container}>
      {ZoneInfoList.map((section) => (
        <Section title={section.GroupName} className={styles.zoneContainer}>
          <Scrollable className={styles.scrollableContainer}>
            <div>
              {section.Zones.map((zone) => (
                <ZoneItem zone={zone}></ZoneItem>
              ))}
            </div>
          </Scrollable>
        </Section>
      ))}
    </div>
  );
};

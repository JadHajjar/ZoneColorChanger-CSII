import { useValue, bindValue } from "cs2/api";
import mod from "../../../mod.json";
import styles from "./OpacitySettings.module.scss";
import { OpacitySlider } from "mods/OpacitySlider/OpacitySlider";
import { Section } from "mods/Section/Section";

const CellFillAlpha$ = bindValue<number>(mod.id, "CellFillAlpha", 0);
const CellEdgeAlpha$ = bindValue<number>(mod.id, "CellEdgeAlpha", 0);
const CellUnzonedFillAlpha$ = bindValue<number>(mod.id, "CellUnzonedFillAlpha", 0);
const CellUnzonedEdgeAlpha$ = bindValue<number>(mod.id, "CellUnzonedEdgeAlpha", 0);

export const OpacitySettings = () => {
  const CellEdgeAlpha = useValue(CellEdgeAlpha$);
  const CellFillAlpha = useValue(CellFillAlpha$);
  const CellUnzonedEdgeAlpha = useValue(CellUnzonedEdgeAlpha$);
  const CellUnzonedFillAlpha = useValue(CellUnzonedFillAlpha$);

  return (
    <div className={styles.optionsContainer}>
      <Section title="Zoned Cell">
        <div className={styles.sliders}>
          <div>
            <OpacitySlider value={CellFillAlpha} trigger="SetCellFillAlpha" />
            <span>Fill Opacity</span>
          </div>
          <div>
            <OpacitySlider value={CellEdgeAlpha} trigger="SetCellEdgeAlpha" />
            <span>Border Opacity</span>
          </div>
        </div>
      </Section>
      <Section title="Empty Cell">
        <div className={styles.sliders}>
          <div>
            <OpacitySlider value={CellUnzonedFillAlpha} trigger="SetUnzonedCellFillAlpha" />
            <span>Fill Opacity</span>
          </div>
          <div>
            <OpacitySlider value={CellUnzonedEdgeAlpha} trigger="SetUnzonedCellEdgeAlpha" />
            <span>Border Opacity</span>
          </div>
        </div>
      </Section>
    </div>
  );
};

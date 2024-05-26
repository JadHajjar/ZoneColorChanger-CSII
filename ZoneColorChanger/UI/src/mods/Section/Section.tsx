import styles from "./Section.module.scss";
import { ReactNode, CSSProperties } from "react";

export const Section = (props: { title?: string; children?: ReactNode; style?: CSSProperties; className?: string }) => {
  return (
    <div className={styles.container + " " + props.className} style={props.style}>
      <span className={styles.title}>{props.title}</span>
      {props.children}
    </div>
  );
};

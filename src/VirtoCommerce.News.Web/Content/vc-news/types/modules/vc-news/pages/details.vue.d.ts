import { IParentCallArgs } from "@vc-shell/framework";
export interface Props {
    expanded?: boolean;
    closable?: boolean;
    param?: string;
}
export interface Emits {
    (event: "parent:call", args: IParentCallArgs): void;
    (event: "collapse:blade"): void;
    (event: "expand:blade"): void;
    (event: "close:blade"): void;
}
declare const _default: import("vue").DefineComponent<Props, {}, {}, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, {} & {
    "parent:call": (args: IParentCallArgs) => any;
    "collapse:blade": () => any;
    "expand:blade": () => any;
    "close:blade": () => any;
}, string, import("vue").PublicProps, Readonly<Props> & Readonly<{
    "onParent:call"?: ((args: IParentCallArgs) => any) | undefined;
    "onCollapse:blade"?: (() => any) | undefined;
    "onExpand:blade"?: (() => any) | undefined;
    "onClose:blade"?: (() => any) | undefined;
}>, {
    expanded: boolean;
    closable: boolean;
    param: string;
}, {}, {}, {}, string, import("vue").ComponentProvideOptions, false, {}, any>;
export default _default;
//# sourceMappingURL=details.vue.d.ts.map
import { RouteRecordRaw } from "vue-router";
import App from "../pages/App.vue";
import { Invite, Login, ResetPassword, ChangePasswordPage, ForgotPassword } from "@vc-shell/framework";
import whiteLogoImage from "/assets/logo-white.svg";
import bgImage from "/assets/background.jpg";

export const routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: App,
    name: "App",
    meta: {
      root: true,
    },
    children: [],
    redirect: (to) => {
      if (to.name === "App") {
        return { path: "/list-all", params: to.params };
      }
      return to.path;
    },
  },
  {
    name: "Login",
    path: "/login",
    component: Login,
    props: () => ({
      logo: whiteLogoImage,
      title: "Vc News",
    }),
  },
  {
    name: "Invite",
    path: "/invite",
    component: Invite,
    props: (_route) => ({
      userId: _route.query.userId,
      token: _route.query.token,
      userName: _route.query.userName,
      logo: whiteLogoImage,
    }),
  },
  {
    name: "ForgotPassword",
    path: "/forgot-password",
    component: ForgotPassword,
    props: () => ({
      logo: whiteLogoImage,
    }),
  },
  {
    name: "ResetPassword",
    path: "/resetpassword",
    component: ResetPassword,
    props: (_route) => ({
      userId: _route.query.userId,
      token: _route.query.token,
      userName: _route.query.userName,
      logo: whiteLogoImage,
    }),
  },
  {
    name: "ChangePassword",
    path: "/changepassword",
    component: ChangePasswordPage,
    meta: {
      forced: true,
    },
  },
];
